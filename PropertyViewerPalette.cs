using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows;

namespace PropertyTable
{
  public class PropertyViewerPalette
  {
    PaletteSet ps;
    static PropertyDataGridView dgv;

    public void InitDataList()
    {
      dgv.InitDataList();
    }
    public void Show()
    {
      if (ps == null)
      {
        ps = new PaletteSet("Property");
        ps.Style = PaletteSetStyles.NameEditable |
                   PaletteSetStyles.ShowPropertiesMenu |
                   PaletteSetStyles.ShowAutoHideButton |
                   PaletteSetStyles.ShowCloseButton;
        ps.Dock = DockSides.Top;
        //ps.MinimumSize = new System.Drawing.Size(150, 300);
        dgv = new PropertyDataGridView();
        ps.Add("Property", dgv);
      }
      else
        Update();
      ps.Visible = true;
    }
    public void Update()
    {
        dgv.UpdateData();
    }

  }
}
