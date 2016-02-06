using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace PropertyTable
{
  public partial class PropertyDataGridView : UserControl
  {
    PropertyDataList data;
    public PropertyDataGridView()
    {
      InitializeComponent();
//      data = new PropertyDataList();
      InitDataList();
    }

    private void dataGridView_SelectionChanged(object sender, EventArgs e)
    {
      PropertyDataList.setEventParentDGV();
      if (PropertyDataList.isEventParentDGV())
          data.SelectEntity();
      PropertyDataList.clearEventParentDGV();
    }
    public void InitDataList()
    {
      Document doc = Application.DocumentManager.MdiActiveDocument;
      Editor ed = doc.Editor;
      Hashtable ud = doc.UserData;

      const string myKey = "PropertyDataList";
      data = ud[myKey] as PropertyDataList;
      if (data == null)
      {
        object obj = ud[myKey];
        if (obj == null)
        {
          // PropertyDataList не найден - первый запуск
          data = new PropertyDataList();
          ud.Add(myKey, data);
          data.dataGridView = dataGridView;
          data.UpdateDataGridView();
        }
        else
        {
          // Найден объект другого типа
          ed.WriteMessage("Найден объект типа \"" + obj.GetType().ToString() + "\" вместо PropertyDataList.");
        }
      }else
      //if (data != null)
      {
        data.Update();
      }
    }
    public void FillData()
     {
       data.Fill();
     }
     public void UpdateData()
     {
       data.Update();
     }

     private void dataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
     {/*
       if (!PropertyDataList.isRunProc)
       {
         PropertyDataList.isRunProc = true;
         data.SelectEntity();
         PropertyDataList.isRunProc = false;
       }
    */
     }

  }
}
