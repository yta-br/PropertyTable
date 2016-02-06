using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;

namespace PropertyTable
{
  public class Commands:IExtensionApplication
  {
    static PropertyViewerPalette pallete;

    public void Initialize()
    {
      Application.DocumentManager.DocumentActivated +=new DocumentCollectionEventHandler(appDocumentActivated);
      Application.SystemVariableChanged += new Autodesk.AutoCAD.ApplicationServices.SystemVariableChangedEventHandler(appSysVarChanged);

    }
    public void appSysVarChanged(object senderObj, Autodesk.AutoCAD.ApplicationServices.SystemVariableChangedEventArgs sysVarChEvtArgs)
    {
      if (sysVarChEvtArgs.Name == "UCSNAME" || sysVarChEvtArgs.Name == "LUPREC" || sysVarChEvtArgs.Name == "LUNITS")
        pallete.Update();
    }
    void appDocumentActivated(object sender, DocumentCollectionEventArgs e)
    {
      if(pallete != null)
        pallete.InitDataList();
    }    
    public void Terminate()
    {
      Application.DocumentManager.DocumentActivated -= new DocumentCollectionEventHandler(appDocumentActivated);
      Application.SystemVariableChanged -= new Autodesk.AutoCAD.ApplicationServices.SystemVariableChangedEventHandler(appSysVarChanged);
    }

    [CommandMethod("pt", CommandFlags.Session)]
    public void PropertyTable()
    {
      if(pallete == null)
        pallete = new PropertyViewerPalette();
      pallete.Show();
    }

  }
}