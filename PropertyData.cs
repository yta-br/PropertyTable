using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.Geometry;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace PropertyTable
{
  public class PropertyData
  {
    //храню только отображаемые в таблице данные, хотя наверное можно хранить и полностью объект(сейчас плюс в том, что данные подкачиваются автоматически в DataGridView)
    public string Handle { get; set; }
    public string Color { get; set; }
    public string Layer { get; set; }
    public string Linetype { get; set; }
    public LineWeight Lineweight { get; set; }
    //public string Lineweight { get; set; }
    
    public string StartPoint { get; set; }
    public string EndPoint { get; set; }
    public Entity ent;

    public PropertyData(ref Entity _ent, EventHandler ModifEntityRow)
    {
      ent = _ent;
      Init();
      ent.Modified += new EventHandler(ModifEntityRow);
    }
    public void Init()
    {
      string startPoint = "";
      string endPoint = "";
      try
      {
        Curve c = ent as Curve;
        Handle = ent.Handle.Value.ToString();
        Color = ent.Color.ToString();
        Layer = ent.Layer.ToString();
        Linetype = ent.Linetype;
        Lineweight = ent.LineWeight;
        if (c != null)
        {
          if (c.StartPoint != null)
            startPoint = FormatPoint(c.StartPoint);
          if (!(ent is Ray || ent is Xline) && c.EndPoint != null)
            endPoint = FormatPoint(c.EndPoint);
        }
        StartPoint = startPoint;
        EndPoint = endPoint;
      }
      catch (System.Exception ex)
      {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Editor ed = doc.Editor;
        ed.WriteMessage("Error: " + ex.Message + "\n" + ex.StackTrace);
      }
    }
    // координаты в UCS и единицах измерения
    public string FormatPoint(Point3d point)
    {
      Document doc = Application.DocumentManager.MdiActiveDocument;
      Editor ed = doc.Editor;
      Matrix3d ucs = ed.CurrentUserCoordinateSystem.Inverse();
      point = point.TransformBy(ucs);
      return (Autodesk.AutoCAD.Runtime.Converter.DistanceToString(point.X) + ", " + Autodesk.AutoCAD.Runtime.Converter.DistanceToString(point.Y) + ", " + Autodesk.AutoCAD.Runtime.Converter.DistanceToString(point.Z));
    }
  }
  public class PropertyDataList : List<PropertyData>
  {
    //public static bool isRunProc; //для разруливания зацикливания вызова друг другом процедур выделения объекта и выделения строки (если одна из них уже запущена, вторая не запускается)
    public DataGridView dataGridView;
    static Document doc;
    static Database db;
    // откуда пришло событие (0-datagrid, 1-entity)
    public enum EventParents {UNDEF = -1, DGV, ENT };
    public static EventParents EventParent;
    public static bool isEventParentDGV()
    {
      return EventParent == EventParents.DGV;
    }
    public static void setEventParentDGV()
    {
      if (EventParent == EventParents.UNDEF)
        EventParent = EventParents.DGV;
    }
    public static void clearEventParentDGV()
    {
      if (EventParent == EventParents.DGV)
        EventParent = EventParents.UNDEF;
    }

    public static bool isEventParentENT()
    {
      return EventParent == EventParents.ENT;
    }
    public static void setEventParentENT()
    {
      if (EventParent == EventParents.UNDEF)
        EventParent = EventParents.ENT;
    }
    public static void clearEventParentENT()
    {
      if (EventParent == EventParents.ENT)
        EventParent = EventParents.UNDEF;
    }

    public PropertyDataList()
    {
      Init();
      Fill();
    }
    void Init() 
    {
      EventParent = EventParents.UNDEF;
      // Получить текущий документ и базу данных
      doc = Application.DocumentManager.MdiActiveDocument;
      db = doc.Database;
    }
    /*
    public EventParents EventParent(EventParents _EventParent)
    {
      if (EventParent == EventParents.UNDEF)
        EventParent = _EventParent;
      return EventParent;
    }
    */
    public void UpdateDataGridView()
    {
        dataGridView.DataSource = null;
        dataGridView.DataSource = this;
    }

    // заполнить таблицу свойств по объектам
    public void Fill()
    {
      try
      {
        doc.ImpliedSelectionChanged += new EventHandler(doc_EntitySelectionChanged);
        db.ObjectAppended += new ObjectEventHandler(db_AddEntityRow);

        this.Clear();

        // начинаем транзакцию
        using (Transaction trn = db.TransactionManager.StartTransaction())
        {
          // Открываем Block table record для чтения
          BlockTable bt = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;

          foreach (ObjectId btrId in bt)
          {
            BlockTableRecord btr = btrId.GetObject(OpenMode.ForRead) as BlockTableRecord;
            BlockTableRecordEnumerator enumer = btr.GetEnumerator();
            while (enumer.MoveNext())
            {
              Entity ent = trn.GetObject(enumer.Current, OpenMode.ForRead) as Entity;

              // событие на изменение объекта
              this.Add(new PropertyData(ref ent, ent_ModifEntityRow));
            }                                            
          }
          trn.Commit();
        }
      }
      catch (System.Exception ex)
      {
        doc.Editor.WriteMessage("Exception: " + ex);
      }
    }
    // обновить данные в таблице свойств
    public void Update()
    {

      Init();
      foreach (PropertyData d in this)
        d.Init();
      UpdateDataGridView();
    
    }
    // выбрать строку с данными по объекту
    public void SelectEntityRow(string strHandle)
    {
        int row = this.FindIndex(o => o.Handle == strHandle);
        SelectRow(row);
    }
    // выбрать строку с данными по объекту
    public void SelectRow(int row, bool clear=false)
    {
      int lastRow = dataGridView.RowCount-1;
      //if (!isRunProc)
      {
      if (/*row == -1 ||*/ row > lastRow)
        row = lastRow; //последняя строка
      if (clear)
      {
        dataGridView.ClearSelection();
      }
      else if (dataGridView.Rows[row].Selected)
      {
        return;
      }
      if (row >= 0 && row <= lastRow)
      {
        dataGridView.Rows[row].Selected = true;
        dataGridView.FirstDisplayedScrollingRowIndex = row;
      }
      }

    }

    // добавить строку с данными по объекту
  private void db_AddEntityRow(object sender, ObjectEventArgs e)
  {
    PropertyDataList.setEventParentENT();
    //if (PropertyDataList.isEventParentENT())
    {
      if ((e.DBObject is Entity) && !(e.DBObject is BlockBegin) && !(e.DBObject is BlockEnd) && (e.DBObject.OwnerId == db.CurrentSpaceId && !e.DBObject.IsErased && e.DBObject.ObjectId != ObjectId.Null))
      {
        Entity ent = e.DBObject as Entity;
        ent.Modified += new EventHandler(ent_ModifEntityRow);
      }
    }
/*
        if (this.Find(o => o.Handle == ent.Handle.Value.ToString()) == null)
        {
          this.Add(new PropertyData(ref ent, ent_ModifEntityRow));
          UpdateDataGridView();
          SelectRow(-1, true);
        }
      }
    }
    PropertyDataList.clearEventParentENT();*/
   }
   // изменение/удаление строки с данными по объекту
    public void ent_ModifEntityRow(object sender, EventArgs evtArgs)
    {
      PropertyDataList.setEventParentENT();
      if (PropertyDataList.isEventParentENT())
      {
        if (!(sender is BlockBegin) && !(sender is BlockEnd))
        {
          Entity ent = sender as Entity;
          int row = this.FindIndex(o => o.ent == ent);
          if (!(sender as DBObject).IsErased)
            if (row != -1)
              this[row].Init();
            else
              this.Add(new PropertyData(ref ent, ent_ModifEntityRow));
          else
          {
            if (row != -1)
              this.RemoveAt(row);

          }
          UpdateDataGridView();
          SelectRow(row, true);
        }
      }
      PropertyDataList.clearEventParentENT();
    }
    // обработка изменения набора выбора объектов
    private void doc_EntitySelectionChanged(object sender, System.EventArgs e)
    {
      PropertyDataList.setEventParentENT();
      if (PropertyDataList.isEventParentENT())
      {
        PromptSelectionResult res = ((Document)sender).Editor.SelectImplied();
        if (res != null && res.Value != null)
        {
          ObjectId[] ids = res.Value.GetObjectIds();
          if (ids != null)
          {
            foreach (ObjectId objID in ids)
            {
              SelectEntityRow(objID.Handle.Value.ToString());
            }
          }
        }
        else
        {
          dataGridView.ClearSelection();
        }
      }
      PropertyDataList.clearEventParentENT();
    }
    //приблизить выделенный
    private static void ZoomWin(Editor ed, Point3d min, Point3d max)
    {
      Point2d min2d = new Point2d(min.X, min.Y);
      Point2d max2d = new Point2d(max.X, max.Y);

      ViewTableRecord view = new ViewTableRecord();

      view.CenterPoint = min2d + ((max2d - min2d) / 2.0);
      view.Height = max2d.Y - min2d.Y;
      view.Width = max2d.X - min2d.X;
      ed.SetCurrentView(view);
    }
    //выбрать объект на чертеже
    public void SelectEntity()
    {
      Editor ed = doc.Editor;
      try
      {
        if (dataGridView.SelectedRows.Count > 0)
        {
          ObjectId[] ids = new ObjectId[dataGridView.SelectedRows.Count];
          int i = 0;
          Entity ent = null;
          foreach (DataGridViewRow r in dataGridView.SelectedRows)
          {
            ent = this[r.Index].ent;
            if (ent != null)
            {
              ids[i] = ent.ObjectId;
              i++;
            }
          }
          ed.SetImpliedSelection(ids);
          ed.Regen();
/*
          Extents3d ext;
          //приблизить выделеное, если выделено несколько, приблизится последний
          if (ent != null)
          {
            ext = ent.GeometricExtents;
            ext.TransformBy(ed.CurrentUserCoordinateSystem.Inverse());
            ZoomWin(ed, ext.MinPoint, ext.MaxPoint);
          }  */
        }
      }
      catch (System.Exception ex)
      {
        ed.WriteMessage("Exception: " + ex);
      }
    }
  }
}
