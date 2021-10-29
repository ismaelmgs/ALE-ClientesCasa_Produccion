using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using ClientesCasa.DomainModel;
using ClientesCasa.Interfaces;
using ClientesCasa.Objetos;
using ClientesCasa.Presenter;

using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Configuration;

//using Microsoft.Office.Interop.Excel;
using NucleoBase.Core;
using System.Globalization;
using ClientesCasa.Clases;
using System.Xml;
using System.Text.RegularExpressions;

namespace ClientesCasa.Views.ASC
{
    public partial class frmPolizaNomina : System.Web.UI.Page, IViewPolizaNomina
    {
        #region EVENTOS
        protected void Page_Load(object sender, EventArgs e)
        {
            oPresenter = new PolizaNomina_Presenter(this, new DBPolizaNomina());

            if (!IsPostBack) 
            { }
        }

        protected void btnLeer_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate("groupFechas");

                if (Page.IsValid)
                {
                    if ((!string.IsNullOrEmpty(txtFechaInicio.Text) && !string.IsNullOrEmpty(txtFechaFinal.Text)))
                    {
                        if (txtFechaInicio.Text.Dt() < txtFechaFinal.Text.Dt())
                        {
                            msgError.Visible = false;
                            lblError.Text = string.Empty;
                            msgSuccesss.Visible = false;
                            lblSuccess.Text = string.Empty;

                            ValidarFileUpload();
                        }
                        else
                        {
                            //MostrarMensaje("Ingrese un período válido.", "Lo sentimos. Intente nuevamente.");
                            msgError.Visible = true;
                            lblError.Text = "Ingrese un período válido.";
                            msgSuccesss.Visible = false;
                            lblSuccess.Text = string.Empty;
                        }
                    }
                    else
                    {
                        //MostrarMensaje("Ingrese una fecha en los campos solicitados.", "Lo sentimos. Intente nuevamente.");
                        msgError.Visible = true;
                        lblError.Text = "Ingrese una fecha en los campos solicitados.";
                        msgSuccesss.Visible = false;
                        lblSuccess.Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: ", ex.Message.ToString());
            }
        }

        protected void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnValidacion.Value == "1")
                {
                    InsertarDatos(dtExcel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region MÉTODOS
        protected void ValidarFileUpload()
        {
            try
            {
                if (fluArchivo.HasFile == true)
                {
                    string FileName = Path.GetFileName(fluArchivo.PostedFile.FileName);
                    string Extension = Path.GetExtension(fluArchivo.PostedFile.FileName);
                    string FolderPath = "~/Files/";
                    string FilePath = Server.MapPath(FolderPath + FileName);
                    sArchivoExcel = FileName;

                    if (!string.IsNullOrEmpty(FilePath))
                        sArchivoSimulado = FilePath;

                    if (Extension == ".xls" || Extension == ".xlsx" || Extension == ".XLS" || Extension == ".XLSX")
                    {
                        if (File.Exists(FilePath))
                            File.Delete(FilePath);

                        if (!File.Exists(FilePath))
                            fluArchivo.SaveAs(FilePath);

                        ImportToDataTable(FilePath, Extension);
                        File.Delete(FilePath);
                    }
                    else
                    {
                        msgError.Visible = true;
                        lblError.Text = "El tipo de archivo a procesar no es válido, se recomienda subir archivos Excel.";
                    }
                }
                else
                {
                    msgError.Visible = true;
                    lblError.Text = "No ha seleccionado archivo a procesar, favor de verificar.";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ImportToDataTable(string FilePath, string Extension) 
        {
            try
            {
                msgError.Visible = false;
                msgSuccesss.Visible = false;
                lblError.Text = string.Empty;
                lblSuccess.Text = string.Empty;
                string conStr = "";
                string SheetName = string.Empty;
                bool bValidExcel = false;
                bool bValidacion = false;

                switch (Extension)
                {
                    case ".xls": //Excel 97-03
                        conStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\";";
                        break;
                    case ".xlsx": //Excel 07, 2013, etc
                        conStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
                        break;
                    case ".XLS": //Excel 97-03
                        conStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\";";
                        break;
                    case ".XLSX": //Excel 07, 2013, etc
                        conStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
                        break;
                }

                if (!string.IsNullOrEmpty(conStr))
                {
                    conStr = String.Format(conStr, FilePath);
                    OleDbConnection connExcel = new OleDbConnection(conStr);
                    OleDbCommand cmdExcel = new OleDbCommand();
                    OleDbDataAdapter oda = new OleDbDataAdapter();

                    System.Data.DataTable dt = new System.Data.DataTable();
                    cmdExcel.Connection = connExcel;
                    //Obtiene el nombre de la primera hoja
                    connExcel.Open();
                    System.Data.DataTable dtExcelSchema;
                    dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                    connExcel.Close();

                    string sQuery = string.Empty;
                    sQuery = "SELECT * FROM [" + SheetName + "]";
                    cmdExcel.CommandText = sQuery;
                    oda.SelectCommand = cmdExcel;
                    oda.Fill(dt);
                    //dtExcel = dt;

                    #region FORMATEAR DATATABLE

                    //Elimina las columnas en blanco despues de la columna TOTAL
                    int desiredSize = 116;

                    while (dt.Columns.Count > desiredSize)
                    {
                        dt.Columns.RemoveAt(desiredSize);
                    }

                    dtExcel = FormatterData(dt);

                    #endregion

                    if (dtExcel.Columns.Count == 116)
                    {
                        #region Valida columnas y formato de Layout a Cargar
                        bValidExcel = ValidarArchivo(dtExcel, "1");
                        #endregion
                        bool bValidaCampos = false;

                        if (bValidExcel)
                        {
                            bValidaCampos = ValidaDatosDataTable(dtExcel);

                            if (bValidaCampos)
                            {
                                pnlBotonesProcesar.Visible = true;
                                hdnValidacion.Value = "1";
                                btnInsertar.Enabled = true;
                                gvDatos.DataSource = dtExcel;
                                gvDatos.DataBind();
                            }
                            else
                            {
                                pnlBotonesProcesar.Visible = false;
                                hdnValidacion.Value = "0";
                                btnInsertar.Enabled = false;
                            }

                        }
                    }
                    else
                    {
                        msgSuccesss.Visible = false;
                        lblSuccess.Visible = false;
                        msgError.Visible = true;
                        lblError.Visible = true;
                        lblError.Text = "No se puede leer el archivo, verifique las columnas.";
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected DataTable FormatterData(DataTable dt) 
        {
            try
            {
                dt.Columns[0].ColumnName = "Empresa";
                dt.Columns[1].ColumnName = "Periodo";
                dt.Columns[2].ColumnName = "Fecha";
                dt.Columns[3].ColumnName = "TipoMovimiento";
                dt.Columns[4].ColumnName = "Factura";
                dt.Columns[5].ColumnName = "TipoNomina";
                dt.Columns[6].ColumnName = "Nombre";
                dt.Columns[7].ColumnName = "RFC";
                dt.Columns[8].ColumnName = "SalarioMensual";
                dt.Columns[9].ColumnName = "Vales";
                dt.Columns[10].ColumnName = "HorasExtras";
                dt.Columns[11].ColumnName = "HorasExtrasTLC";
                dt.Columns[12].ColumnName = "HorasExtrasADN";
                dt.Columns[13].ColumnName = "HorasExtrasPend";
                dt.Columns[14].ColumnName = "IncidenciasNoRep";
                dt.Columns[15].ColumnName = "Faltas";
                dt.Columns[16].ColumnName = "Devolucion"; //Agregada
                dt.Columns[17].ColumnName = "DevolucionCredInfo";
                dt.Columns[18].ColumnName = "IncidenciasQ3MexJet"; //Agregada
                dt.Columns[19].ColumnName = "PagoPorIncapacidad";
                dt.Columns[20].ColumnName = "Compensacion";
                dt.Columns[21].ColumnName = "CompensacionPorRetro";
                dt.Columns[22].ColumnName = "CompensacionFijaP114";
                dt.Columns[23].ColumnName = "DevolucionPorFaltas"; //Agregado
                dt.Columns[24].ColumnName = "DevolucionPorPrestamo"; //Agregado
                dt.Columns[25].ColumnName = "Aguinaldo"; //Agregado
                dt.Columns[26].ColumnName = "DiasLaborados"; //Modificado Posicion
                dt.Columns[27].ColumnName = "SD"; //Agregado
                dt.Columns[28].ColumnName = "PagoPropina"; //Modificado Posicion
                dt.Columns[29].ColumnName = "Retroactivo";
                dt.Columns[30].ColumnName = "HorasDiasFestivos";
                dt.Columns[31].ColumnName = "HorasDiasFestivosTLC";
                dt.Columns[32].ColumnName = "HorasDiasFestivosADN";
                dt.Columns[33].ColumnName = "PrimaDominical";
                dt.Columns[34].ColumnName = "PrimaDominicalTLC";
                dt.Columns[35].ColumnName = "PrimaDominicalADN";
                dt.Columns[36].ColumnName = "PrimaVacacionalTLC";
                dt.Columns[37].ColumnName = "PrimaVacacionalADN"; //Agregado
                dt.Columns[38].ColumnName = "PrimaVacacional";
                dt.Columns[39].ColumnName = "INCXAntiguedad"; //Agregado
                dt.Columns[40].ColumnName = "PrimaAntiguedad";
                dt.Columns[41].ColumnName = "Bono";
                dt.Columns[42].ColumnName = "TotalIngresos";
                dt.Columns[43].ColumnName = "SalarioDiario";
                dt.Columns[44].ColumnName = "SalarioIntegrado";
                dt.Columns[45].ColumnName = "Sueldo";
                dt.Columns[46].ColumnName = "SeptimoDia";
                dt.Columns[47].ColumnName = "HorasExtras2";
                dt.Columns[48].ColumnName = "Destajos";
                dt.Columns[49].ColumnName = "PremioEficiencia";
                dt.Columns[50].ColumnName = "Vacaciones";
                dt.Columns[51].ColumnName = "PrimaVacacional2";
                dt.Columns[52].ColumnName = "Aguinaldo2";
                dt.Columns[53].ColumnName = "OtrasPercepciones";
                dt.Columns[54].ColumnName = "TotalPercepciones";
                dt.Columns[55].ColumnName = "RetInvVida";
                dt.Columns[56].ColumnName = "RetCesantia";
                dt.Columns[57].ColumnName = "RetEnfMatObrero";
                dt.Columns[58].ColumnName = "SeguroViviendaInfonavit";
                dt.Columns[59].ColumnName = "SubsEmpleoAcreditado";
                dt.Columns[60].ColumnName = "SubsidioEmpleo";
                dt.Columns[61].ColumnName = "ISRAntesSubsEmpleo";
                dt.Columns[62].ColumnName = "ISR_Art142"; //Agregado
                dt.Columns[63].ColumnName = "ISR_SP";
                dt.Columns[64].ColumnName = "IMSS";
                dt.Columns[65].ColumnName = "PrestamoInfonavit";
                dt.Columns[66].ColumnName = "PrestamoEmpresa"; //Modificado Posicion
                dt.Columns[67].ColumnName = "AjusteNeto";
                dt.Columns[68].ColumnName = "PensionAlimenticia";
                dt.Columns[69].ColumnName = "OtrasDeducciones";
                dt.Columns[70].ColumnName = "TotalDeducciones";
                dt.Columns[71].ColumnName = "Neto";
                dt.Columns[72].ColumnName = "NetoIMSSReal"; //Agregado
                dt.Columns[73].ColumnName = "ValesGratificacion";
                dt.Columns[74].ColumnName = "InvalidezVida";
                dt.Columns[75].ColumnName = "CesantiaVejez";
                dt.Columns[76].ColumnName = "EnfMatPatron";
                dt.Columns[77].ColumnName = "FondoRetiroSAR2PorCiento";
                dt.Columns[78].ColumnName = "ImpuestoEstatal3PorCiento";
                dt.Columns[79].ColumnName = "RiesgoTrabajo";
                dt.Columns[80].ColumnName = "IMSSEmpresa";
                dt.Columns[81].ColumnName = "InfonavitEmpresa";
                dt.Columns[82].ColumnName = "GuarderiaIMSS";
                dt.Columns[83].ColumnName = "OtrasObligaciones";
                dt.Columns[84].ColumnName = "TotalObligaciones";
                dt.Columns[85].ColumnName = "EmpresaAsimilados"; //Nombre Modificado
                dt.Columns[86].ColumnName = "Asimilados";
                dt.Columns[87].ColumnName = "ISR";
                dt.Columns[88].ColumnName = "TotalPagarAsimiladosSalario";
                dt.Columns[89].ColumnName = "PrestamoCompania";
                dt.Columns[90].ColumnName = "InteresesPrestamo";
                dt.Columns[91].ColumnName = "RecuperacionAsesores";
                dt.Columns[92].ColumnName = "GastosNoComprobados"; //Agregado
                dt.Columns[93].ColumnName = "DescuentoGMM";
                dt.Columns[94].ColumnName = "CursoIngles"; //Agregado
                dt.Columns[95].ColumnName = "DescuentoSportium";
                dt.Columns[96].ColumnName = "DescuentoPorMerma";
                dt.Columns[97].ColumnName = "DescuentoPorPagoDeMas"; //Agregado
                dt.Columns[98].ColumnName = "DescuentoPorFaltas"; //Modificado Posicion
                dt.Columns[99].ColumnName = "GastosNoComprobados2";
                dt.Columns[100].ColumnName = "Ayudante";
                dt.Columns[101].ColumnName = "DescuentoClienteOtros";
                dt.Columns[102].ColumnName = "Otros";
                dt.Columns[103].ColumnName = "PensionAlimencia2";
                dt.Columns[104].ColumnName = "NetoPagarAsimilados";
                dt.Columns[105].ColumnName = "TarjetaEmpresarialISR"; //Agregado
                dt.Columns[106].ColumnName = "SueldoIMSS";
                dt.Columns[107].ColumnName = "AsimiladosSalarios";
                dt.Columns[108].ColumnName = "TarjetaEmpresarialISR2"; //Agregado
                dt.Columns[109].ColumnName = "CargaPatronal";
                dt.Columns[110].ColumnName = "MARKUP7_5Porciento";
                dt.Columns[111].ColumnName = "FactorChequeCertSub";
                dt.Columns[112].ColumnName = "CostoEmpleado";
                dt.Columns[113].ColumnName = "Importe";
                dt.Columns[114].ColumnName = "IVA";
                dt.Columns[115].ColumnName = "TotalFactura";
                dt.AcceptChanges();
                return dt;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        protected bool ValidaDatosDataTable(DataTable dt)
        {
            try
            {
                ViewState["vsDataTable"] = null;
                msgSuccesss.Visible = false;
                lblSuccess.Text = string.Empty;
                msgError.Visible = false;
                lblError.Text = string.Empty;

                int iFila = 0;
                string strCampo = string.Empty;
                string strValor = string.Empty;
                string strExcepcion = string.Empty;
                int iStatus = 0;

                foreach (DataRow dRow in dt.Rows)
                {
                    iStatus = 1;
                    iFila = dt.Rows.IndexOf(dRow) + 2;

                    if (!string.IsNullOrEmpty(dRow["RFC"].S()))
                    {
                        sRFC = dRow["RFC"].S();

                        if (eSearchRFC != null)
                            eSearchRFC(null, null);

                        if (dtExistsRFC.Rows[0][0].S().I() == 0)
                        {
                            strCampo = "RFC";
                            strValor = dRow["RFC"].S();
                            strExcepcion = "El RFC '" + strValor + "' no existe en la tabla [ALE_RH].";
                            iStatus = 0;
                        }
                    }
                    else
                    {
                        strCampo = "RFC";
                        strValor = dRow["RFC"].S();
                        strExcepcion = "El campo 'RFC' viene vacio, favor de verificar";
                        iStatus = 0;
                    }

                    if (iStatus == 0)
                        dtRow(iFila, strCampo, strValor, iStatus, strExcepcion);
                }

                gvResultado.DataSource = (System.Data.DataTable)ViewState["vsDataTable"];
                gvResultado.DataBind();

                if (gvResultado.Rows.Count > 0 && (System.Data.DataTable)ViewState["vsDataTable"] != null)
                {
                    int rowCount = gvResultado.Rows.Count;

                    if (rowCount > 0)
                    {
                        msgError.Visible = true;
                        lblError.Text = "El archivo contiene algunos errores que impiden su procesamiento.";
                        msgSuccesss.Visible = false;
                        lblSuccess.Text = string.Empty;
                        btnInsertar.Enabled = false;
                        return false;
                    }
                    return false;
                }
                else
                {
                    msgSuccesss.Visible = true;
                    lblSuccess.Text = "El archivo cumple los requerimientos.";
                    msgError.Visible = false;
                    lblError.Text = string.Empty;
                    btnInsertar.Enabled = true;
                    return true;
                }

                //return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void InsertarDatos(DataTable dt) 
        {
            try
            {
                string sNombreArchivo = string.Empty;
                string sFechaIni = string.Empty;
                string sFechaFin = string.Empty;
                string sUsuario = string.Empty;

                //Datos de Header
                sNombreArchivo = sArchivoExcel;
                sFechaIni = txtFechaInicio.Text.Dt().ToShortDateString();
                sFechaFin = txtFechaFinal.Text.Dt().ToShortDateString();
                sUsuario = Utils.GetUserName;
                List<PolizaNomina> oLsPoliza = new List<PolizaNomina>();

                //Datos de Detalle
                if (dt != null && dt.Rows.Count > 0) 
                {
                    //Llenado de Header
                    PolizaNomina oPoliza = new PolizaNomina();
                    oPoliza.sArchivo = sNombreArchivo;
                    oPoliza.dtFechaIni = sFechaIni.Dt();
                    oPoliza.dtFechaFin = sFechaFin.Dt();
                    oPoliza.sUsuario = sUsuario;

                    //Llenado de Detalle
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DetallePolizaNomina oDetalle = new DetallePolizaNomina();
                        oDetalle.sEmpresa = dt.Rows[i]["Empresa"].S();
                        oDetalle.sPeriodo = dt.Rows[i]["Periodo"].S();
                        oDetalle.sFecha = dt.Rows[i]["Fecha"].S();
                        oDetalle.sTipoMovimiento = dt.Rows[i]["TipoMovimiento"].S();
                        oDetalle.sFactura = dt.Rows[i]["Factura"].S();
                        oDetalle.sTipoNomina = dt.Rows[i]["TipoNomina"].S();
                        oDetalle.sNombre = dt.Rows[i]["Nombre"].S();
                        oDetalle.sRFC = dt.Rows[i]["RFC"].S();
                        oDetalle.dSalarioMensual = dt.Rows[i]["SalarioMensual"].S().Replace("$", "").D();
                        oDetalle.dVales = dt.Rows[i]["Vales"].S().Replace("$", "").D();
                        oDetalle.dHorasExtras = dt.Rows[i]["HorasExtras"].S().Replace("$", "").D();
                        oDetalle.dHorasExtraTLC = dt.Rows[i]["HorasExtrasTLC"].S().Replace("$", "").D();
                        oDetalle.dHorasExtraADN = dt.Rows[i]["HorasExtrasADN"].S().Replace("$", "").D();
                        oDetalle.dHorasExtraPend = dt.Rows[i]["HorasExtrasPend"].S().Replace("$", "").D();
                        oDetalle.dIncidenciasNoRep = dt.Rows[i]["IncidenciasNoRep"].S().Replace("$", "").D();
                        oDetalle.dFaltas = dt.Rows[i]["Faltas"].S().Replace("$", "").D();
                        oDetalle.dDevolucion = dt.Rows[i]["Devolucion"].S().Replace("$", "").D();
                        oDetalle.dDevolucionCredInfo = dt.Rows[i]["DevolucionCredInfo"].S().Replace("$", "").D();
                        oDetalle.dIncidenciasQ3MexJet = dt.Rows[i]["IncidenciasQ3MexJet"].S().Replace("$", "").D();
                        oDetalle.dPagoIncapacidad = dt.Rows[i]["PagoPorIncapacidad"].S().Replace("$", "").D();
                        oDetalle.dCompensacion = dt.Rows[i]["Compensacion"].S().Replace("$", "").D();
                        oDetalle.dCompensacionPorRetro = dt.Rows[i]["CompensacionPorRetro"].S().Replace("$", "").D();
                        oDetalle.dCompensacionFija = dt.Rows[i]["CompensacionFijaP114"].S().Replace("$", "").D();
                        oDetalle.dDevolucionPorFaltas = dt.Rows[i]["DevolucionPorFaltas"].S().Replace("$", "").D();
                        oDetalle.dDevolucionPorPrestamo = dt.Rows[i]["DevolucionPorPrestamo"].S().Replace("$", "").D();
                        oDetalle.dAguinaldo = dt.Rows[i]["Aguinaldo"].S().Replace("$", "").D();
                        oDetalle.dDiasLaborados = dt.Rows[i]["DiasLaborados"].S().Replace("$", "").D();
                        oDetalle.dSD = dt.Rows[i]["SD"].S().Replace("$", "").D();
                        oDetalle.dPagoPropina = dt.Rows[i]["PagoPropina"].S().Replace("$", "").D();
                        oDetalle.dRetroactivo = dt.Rows[i]["Retroactivo"].S().Replace("$", "").D();
                        oDetalle.dHorasDiasFestivos = dt.Rows[i]["HorasDiasFestivos"].S().Replace("$", "").D();
                        oDetalle.dHorasFestivosTLC = dt.Rows[i]["HorasDiasFestivosTLC"].S().Replace("$", "").D();
                        oDetalle.dHorasFestivosADN = dt.Rows[i]["HorasDiasFestivosADN"].S().Replace("$", "").D();
                        oDetalle.dPrimaDominical = dt.Rows[i]["PrimaDominical"].S().Replace("$", "").D();
                        oDetalle.dPrimaDominicalTLC = dt.Rows[i]["PrimaDominicalTLC"].S().Replace("$", "").D();
                        oDetalle.dPrimaDominicalADN = dt.Rows[i]["PrimaDominicalADN"].S().Replace("$", "").D();
                        oDetalle.dPrimaVacacionalTLC = dt.Rows[i]["PrimaVacacionalTLC"].S().Replace("$", "").D();
                        oDetalle.dPrimaVacacionalADN = dt.Rows[i]["PrimaVacacionalADN"].S().Replace("$", "").D();
                        oDetalle.dPrimaVacacional = dt.Rows[i]["PrimaVacacional"].S().Replace("$", "").D();
                        oDetalle.dINCXAntiguedad = dt.Rows[i]["INCXAntiguedad"].S().Replace("$", "").D();
                        oDetalle.dPrimaAntiguedad = dt.Rows[i]["PrimaAntiguedad"].S().Replace("$", "").D();
                        oDetalle.dBono = dt.Rows[i]["Bono"].S().Replace("$", "").D();
                        oDetalle.dTotalIngresos = dt.Rows[i]["TotalIngresos"].S().Replace("$", "").D();
                        oDetalle.dSalarioDiario = dt.Rows[i]["SalarioDiario"].S().Replace("$", "").D();
                        oDetalle.dSalarioIntegrado = dt.Rows[i]["SalarioIntegrado"].S().Replace("$", "").D();
                        oDetalle.dSueldo = dt.Rows[i]["Sueldo"].S().Replace("$", "").D();
                        oDetalle.dSeptimoDia = dt.Rows[i]["SeptimoDia"].S().Replace("$", "").D();
                        oDetalle.dHorasExtras2 = dt.Rows[i]["HorasExtras2"].S().Replace("$", "").D();
                        oDetalle.dDestajos = dt.Rows[i]["Destajos"].S().Replace("$", "").D();
                        oDetalle.dPremioEficiencia = dt.Rows[i]["PremioEficiencia"].S().Replace("$", "").D();
                        oDetalle.dVacaciones = dt.Rows[i]["Vacaciones"].S().Replace("$", "").D();
                        oDetalle.dPrimaVacacional2 = dt.Rows[i]["PrimaVacacional2"].S().Replace("$", "").D();
                        oDetalle.dAguinaldo2 = dt.Rows[i]["Aguinaldo2"].S().Replace("$", "").D();
                        oDetalle.dOtrasPercepciones = dt.Rows[i]["OtrasPercepciones"].S().Replace("$", "").D();
                        oDetalle.dTotalPercepciones = dt.Rows[i]["TotalPercepciones"].S().Replace("$", "").D();
                        oDetalle.dRetInvVida = dt.Rows[i]["RetInvVida"].S().Replace("$", "").D();
                        oDetalle.dRetCesantia = dt.Rows[i]["RetCesantia"].S().Replace("$", "").D();
                        oDetalle.dRetEnfMatObrero = dt.Rows[i]["RetEnfMatObrero"].S().Replace("$", "").D();
                        oDetalle.dSeguroViviendaInfonavit = dt.Rows[i]["SeguroViviendaInfonavit"].S().Replace("$", "").D();
                        oDetalle.dSubsEmpleoAcreditado = dt.Rows[i]["SubsEmpleoAcreditado"].S().Replace("$", "").D();
                        oDetalle.dSubsidioEmpleo = dt.Rows[i]["SubsidioEmpleo"].S().Replace("$", "").D();
                        oDetalle.dISRAntesSubsEmpleo = dt.Rows[i]["ISRAntesSubsEmpleo"].S().Replace("$", "").D();
                        oDetalle.dISR_Art142 = dt.Rows[i]["ISR_Art142"].S().D();
                        oDetalle.dISR_SP = dt.Rows[i]["ISR_SP"].S().Replace("$", "").D();
                        oDetalle.dIMSS = dt.Rows[i]["IMSS"].S().Replace("$", "").D();
                        oDetalle.dPrestamoInfonavit = dt.Rows[i]["PrestamoInfonavit"].S().Replace("$", "").D();
                        oDetalle.dPrestamoEmpresa = dt.Rows[i]["PrestamoEmpresa"].S().Replace("$", "").D();
                        oDetalle.dAjusteNeto = dt.Rows[i]["AjusteNeto"].S().Replace("$", "").D();
                        oDetalle.dPensionAlimenticia = dt.Rows[i]["PensionAlimenticia"].S().Replace("$", "").D();
                        oDetalle.dOtrasDeducciones = dt.Rows[i]["OtrasDeducciones"].S().Replace("$", "").D();
                        oDetalle.dTotalDeducciones = dt.Rows[i]["TotalDeducciones"].S().Replace("$", "").D();
                        oDetalle.dNeto = dt.Rows[i]["Neto"].S().Replace("$", "").D();
                        oDetalle.dNetoIMSSReal = dt.Rows[i]["NetoIMSSReal"].S().Replace("$", "").D();
                        oDetalle.dValesGratificacion = dt.Rows[i]["ValesGratificacion"].S().Replace("$", "").D();
                        oDetalle.dInvalidezVida = dt.Rows[i]["InvalidezVida"].S().Replace("$", "").D();
                        oDetalle.dCesantiaVejez = dt.Rows[i]["CesantiaVejez"].S().Replace("$", "").D();
                        oDetalle.dEnfMatPatron = dt.Rows[i]["EnfMatPatron"].S().Replace("$", "").D();
                        oDetalle.dFondoRetiroSAR2Porciento = dt.Rows[i]["FondoRetiroSAR2PorCiento"].S().Replace("$", "").D();
                        oDetalle.dImpuestoEstatal3Porciento = dt.Rows[i]["ImpuestoEstatal3PorCiento"].S().Replace("$", "").D();
                        oDetalle.dRiesgoTrabajo = dt.Rows[i]["RiesgoTrabajo"].S().Replace("$", "").D();
                        oDetalle.dIMSSEmpresa = dt.Rows[i]["IMSSEmpresa"].S().Replace("$", "").D();
                        oDetalle.dInfonavitEmpresa = dt.Rows[i]["InfonavitEmpresa"].S().Replace("$", "").D();
                        oDetalle.dGuarderiaIMSS = dt.Rows[i]["GuarderiaIMSS"].S().Replace("$", "").D();
                        oDetalle.dOtrasObligaciones = dt.Rows[i]["OtrasObligaciones"].S().Replace("$", "").D();
                        oDetalle.dTotalObligaciones = dt.Rows[i]["TotalObligaciones"].S().Replace("$", "").D();
                        oDetalle.dEmpresaPagaAsimilados = dt.Rows[i]["EmpresaAsimilados"].S().Replace("$", "").D();
                        oDetalle.dAsimilados = dt.Rows[i]["Asimilados"].S().Replace("$", "").D();
                        oDetalle.dISR = dt.Rows[i]["ISR"].S().Replace("$", "").D();
                        oDetalle.dTotalPagarAsimiladosSalario = dt.Rows[i]["TotalPagarAsimiladosSalario"].S().Replace("$", "").D();
                        oDetalle.dPrestamoCompania = dt.Rows[i]["PrestamoCompania"].S().Replace("$", "").D();
                        oDetalle.dInteresesPrestamo = dt.Rows[i]["InteresesPrestamo"].S().Replace("$", "").D();
                        oDetalle.dRecuperacionAsesores = dt.Rows[i]["RecuperacionAsesores"].S().Replace("$", "").D();
                        oDetalle.dGastosNoComprobados = dt.Rows[i]["GastosNoComprobados"].S().Replace("$", "").D();
                        oDetalle.dDescuentoGMM = dt.Rows[i]["DescuentoGMM"].S().Replace("$", "").D();
                        oDetalle.dCursoIngles = dt.Rows[i]["CursoIngles"].S().Replace("$", "").D();
                        oDetalle.dDescuentoSportium = dt.Rows[i]["DescuentoSportium"].S().Replace("$", "").D();
                        oDetalle.dDescuentoPorMerma = dt.Rows[i]["DescuentoPorMerma"].S().Replace("$", "").D();
                        oDetalle.dDescuentoPorPagoDeMas = dt.Rows[i]["DescuentoPorPagoDeMas"].S().Replace("$", "").D();
                        oDetalle.dDescuentoPorFaltas = dt.Rows[i]["DescuentoPorFaltas"].S().Replace("$", "").D();
                        oDetalle.dGastosNoComprobados2 = dt.Rows[i]["GastosNoComprobados2"].S().Replace("$", "").D();
                        oDetalle.dAyudante = dt.Rows[i]["Ayudante"].S().Replace("$", "").D();
                        oDetalle.dDescuentoClienteOtros = dt.Rows[i]["DescuentoClienteOtros"].S().Replace("$", "").D();
                        oDetalle.dOtros = dt.Rows[i]["Otros"].S().Replace("$", "").D();
                        oDetalle.dPensionAlimenticia2 = dt.Rows[i]["PensionAlimencia2"].S().Replace("$", "").D();
                        oDetalle.dNetoPagarAsimilados = dt.Rows[i]["NetoPagarAsimilados"].S().Replace("$", "").D();
                        oDetalle.dTarjetaEmpresarialISR = dt.Rows[i]["TarjetaEmpresarialISR"].S().Replace("$", "").D();
                        oDetalle.dSueldoIMSS = dt.Rows[i]["SueldoIMSS"].S().Replace("$", "").D();
                        oDetalle.dAsimiladosSalarios = dt.Rows[i]["AsimiladosSalarios"].S().Replace("$", "").D();
                        oDetalle.dTarjetaEmpresarialISR2 = dt.Rows[i]["TarjetaEmpresarialISR2"].S().Replace("$", "").D();
                        oDetalle.dCargaPatronal = dt.Rows[i]["CargaPatronal"].S().Replace("$", "").D();
                        oDetalle.dMARKUP7_5Porciento = dt.Rows[i]["MARKUP7_5Porciento"].S().Replace("$", "").D();
                        oDetalle.dFactorChequeCertSub = dt.Rows[i]["FactorChequeCertSub"].S().Replace("$", "").D();
                        oDetalle.dCostoEmpleado = dt.Rows[i]["CostoEmpleado"].S().Replace("$", "").D();
                        oDetalle.dImporte = dt.Rows[i]["Importe"].S().Replace("$", "").D();
                        oDetalle.d_IVA = dt.Rows[i]["IVA"].S().Replace("$", "").D();
                        oDetalle.dTotalFactura = dt.Rows[i]["TotalFactura"].S().Replace("$", "").D();
                        oPoliza.oLstDetalle.Add(oDetalle);
                    }
                    oLsPoliza.Add(oPoliza);
                    ListaPolizas = oLsPoliza;

                    if (eNewProcesaArchivo != null)
                        eNewProcesaArchivo(null, null);

                    if (iSuccess == 1)
                    {
                        msgError.Visible = false;
                        lblError.Visible = false;
                        msgSuccesss.Visible = true;
                        lblSuccess.Visible = true;
                        lblSuccess.Text = "Se cargo correctamente el archivo.";
                    }
                    else
                    {
                        msgSuccesss.Visible = false;
                        lblSuccess.Visible = false;
                        msgError.Visible = true;
                        lblError.Visible = true;
                        lblError.Text = "Error, no se pudo cargar el archivo.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidarArchivo(System.Data.DataTable dsFileExcel, string sLayout)
        {
            try
            {
                int iValidColumn;
                bool bValidExcel = false;
                string[] arrColumn1 = { "Empresa", "Periodo", "TipoMovimiento" };

                switch (sLayout)
                {
                    // Layout Poliza Nomina
                    case "1":
                        ViewState["VSLayout"] = arrColumn1;
                        for (int i = 0; i < arrColumn1.Length; i++)
                        {
                            iValidColumn = dsFileExcel.Columns.IndexOf(arrColumn1[i]);

                            if (iValidColumn == -1)
                            {
                                bValidExcel = false;
                                break;
                            }
                            else
                                bValidExcel = true;
                        }
                        break;
                    default:
                        break;
                }

                return bValidExcel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LoadRFC(DataTable dt) 
        {
            try
            {
                dtExistsRFC = null;
                dtExistsRFC = dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void MostrarMensaje(string sMensaje, string sCaption)
        {
            sMensaje = "alert('" + sMensaje + "');";
            ScriptManager.RegisterClientScriptBlock(this, typeof(System.Web.UI.Page), "alert", sMensaje, true);
        }

        public void ErrorRequireColumns()
        {
            if (ViewState["VSLayout"] != null)
            {
                string[] arrCamposReq = (string[])ViewState["VSLayout"];

                string sMsgRequeridos = string.Empty;

                sMsgRequeridos = @"<ul>";

                for (int i = 0; i < arrCamposReq.Length; i++)
                {
                    sMsgRequeridos += @"<li>" + arrCamposReq[i] + @"</li>";
                }
                sMsgRequeridos += @"</ul>";

                msgError.Visible = true;
                lblError.Text = @"El archivo seleccionado no contiene las columnas requeridas para el proceso, favor de verificar que contenga los siguientes campos:" + sMsgRequeridos;
                //Mensaje(" \n");
            }
            else
            {
                msgError.Visible = false;
                lblError.Text = string.Empty;
            }
        }

        public void dtRow(int iFila, string strCampo, string strValor, int iStatus, string strExcepcion)
        {
            try
            {
                bool bBandera = false;
                System.Data.DataTable dtNew = new System.Data.DataTable();
                System.Data.DataTable dt;

                //Declaramos variables DataColumn y DataRow.
                DataColumn column;
                DataRow row;
                DataView view;

                if (ViewState["vsDataTable"] != null)
                    bBandera = true;
                //else if (System.Web.HttpContext.Current.Session["SErrores"] != null)
                //    bBandera = true;

                // Verificamos si nuestro DataTable en la variable de estado contiene datos
                if (bBandera == false)
                {
                    // Creación de nueva DataColumn, typo de dato y Nombre de la columna.
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.Int32");
                    column.ColumnName = "Fila";
                    dtNew.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Campo";
                    dtNew.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "Valor";
                    dtNew.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = Type.GetType("System.String");
                    column.ColumnName = "Status";
                    dtNew.Columns.Add(column);

                    column = new DataColumn();
                    column.DataType = Type.GetType("System.String");
                    column.ColumnName = "Descripcion";
                    dtNew.Columns.Add(column);

                    // Crea un nuevo objeto DataRow y lo agrega al DataTable. 
                    row = dtNew.NewRow();

                    row["Fila"] = iFila;
                    row["Campo"] = strCampo;
                    row["Valor"] = strValor;

                    if (iStatus == 0)
                        row["Status"] = "Error";
                    else
                        row["Status"] = "Válido";

                    row["Descripcion"] = strExcepcion;
                    dtNew.Rows.Add(row);

                    ViewState["vsDataTable"] = dtNew;
                    //System.Web.HttpContext.Current.Session["SErrores"] = dtNew;
                    view = new DataView(dtNew);
                    pnlSimulacion.Visible = true;
                }
                else
                {
                    dt = (System.Data.DataTable)ViewState["vsDataTable"];
                    //dt = (System.Data.DataTable)System.Web.HttpContext.Current.Session["SErrores"];
                    row = dt.NewRow();

                    row["Fila"] = iFila;
                    row["Campo"] = strCampo;
                    row["Valor"] = strValor;

                    if (iStatus == 0)
                        row["Status"] = "Error";
                    else
                        row["Status"] = "Válido";

                    row["Descripcion"] = strExcepcion;
                    dt.Rows.Add(row);
                    ViewState["vsDataTable"] = dt;
                    //System.Web.HttpContext.Current.Session["SErrores"] = dt;
                    view = new DataView(dt);
                    pnlSimulacion.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
               server control at run time. */
        }

        public void EliminarProceso()
        {
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("Excel");
            foreach (System.Diagnostics.Process p in process)
            {
                if (!string.IsNullOrEmpty(p.ProcessName) && p.StartTime.AddSeconds(+120) > DateTime.Now)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch { }
                }
            }
        }
        #endregion

        #region VARIABLES Y PROPIEDADES
        private const string sClase = "frmPolizaNomina.aspx.cs";
        private const string sPagina = "frmPolizaNomina.aspx";

        PolizaNomina_Presenter oPresenter;
        public event EventHandler eNewObj;
        public event EventHandler eObjSelected;
        public event EventHandler eSaveObj;
        public event EventHandler eDeleteObj;
        public event EventHandler eSearchObj;
        public event EventHandler eNewProcesaArchivo;
        public event EventHandler eSearchRFC;

        public string sArchivoSimulado
        {
            get { return (string)ViewState["VSArchivoSimulado"]; }
            set { ViewState["VSArchivoSimulado"] = value; }
        }
        public string sArchivoExcel
        {
            get { return (string)ViewState["VSArchivoExcel"]; }
            set { ViewState["VSArchivoExcel"] = value; }
        }
        public string sRFC
        {
            get { return (string)ViewState["VSRFC"]; }
            set { ViewState["VSRFC"] = value; }
        }
        public int iSuccess
        {
            get { return (int)ViewState["VSSuccess"]; }
            set { ViewState["VSSuccess"] = value; }
        }
        public DataTable dtExcel
        {
            get { return (DataTable)ViewState["VSdtExcel"]; }
            set { ViewState["VSdtExcel"] = value; }
        }
        public DataTable dtExistsRFC
        {
            get { return (DataTable)ViewState["VSExistsRFC"]; }
            set { ViewState["VSExistsRFC"] = value; }
        }

        public List<PolizaNomina> ListaPolizas
        {
            set { ViewState["VSListaPolizas"] = value; }
            get { return (List<PolizaNomina>)ViewState["VSListaPolizas"]; }
        }
        #endregion

        

        
    }
}