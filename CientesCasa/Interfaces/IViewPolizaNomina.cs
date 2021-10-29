using ClientesCasa.Objetos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientesCasa.Interfaces
{
    public interface IViewPolizaNomina : IBaseView
    {
        List<PolizaNomina> ListaPolizas { set; get; }
        
        //Bandera para saber si se insertó correctamente
        int iSuccess { get; set; }

        string sRFC { get; set; }

        void LoadRFC(DataTable dt);

        event EventHandler eNewProcesaArchivo;
        event EventHandler eSearchRFC;
    }
}
