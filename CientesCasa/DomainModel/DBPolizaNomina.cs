using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClientesCasa.Clases;
using System.Data;
using NucleoBase.Core;
using ClientesCasa.Objetos;
using System.Globalization;
using System.Data.SqlClient;

namespace ClientesCasa.DomainModel
{
    public class DBPolizaNomina : DBIntegrator
    {
        public DataTable GetQuerySAP(string sQuery)
        {
            try
            {
                return oDB_SP.EjecutarDT_DeQuery(sQuery);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DBInsertaRegistrosHeader(PolizaNomina oPolNom) 
        {
            try
            {
                object oRes = new DBIntegrator().oDB_SP.EjecutarValor("[Principales].[spI_DI_InsertaHeaderPolizaNomina]", "@Archivo", oPolNom.sArchivo,
                                                                                                        "@FechaIni", oPolNom.dtFechaIni,
                                                                                                        "@FechaFin", oPolNom.dtFechaFin,
                                                                                                        "@Usuario", oPolNom.sUsuario);
                return oRes.S().I();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DBInsertaPolizaNominaDetalle(List<DetallePolizaNomina> oLstDetails, long lgFolio) 
        {
            try
            {
                foreach (DetallePolizaNomina oDet in oLstDetails)
                {
                    object oRes = new DBIntegrator().oDB_SP.EjecutarValor("[Principales].[spI_DI_InsertaDetallePolizaNomina]",  "@IdFolio", lgFolio,
                                                                                                                                "@Empresa", oDet.sEmpresa,
                                                                                                                                "@Periodo", oDet.sPeriodo,
                                                                                                                                "@Fecha", oDet.sFecha,
                                                                                                                                "@TipoMovimiento", oDet.sTipoMovimiento,
                                                                                                                                "@Factura", oDet.sFactura,
                                                                                                                                "@TipoNomina", oDet.sTipoNomina,
                                                                                                                                "@Nombre", oDet.sNombre,
                                                                                                                                "@RFC", oDet.sRFC,
                                                                                                                                "@SalarioMensual", oDet.dSalarioMensual,
                                                                                                                                "@Vales", oDet.dVales,
                                                                                                                                "@HorasExtras", oDet.dHorasExtras,
                                                                                                                                "@HorasExtrasTLC", oDet.dHorasExtraTLC,
                                                                                                                                "@HorasExtrasADN", oDet.dHorasExtraADN,
                                                                                                                                "@HorasExtrasPend", oDet.dHorasExtraPend,
                                                                                                                                "@IncidenciasNoRep", oDet.dIncidenciasNoRep,
                                                                                                                                "@Faltas", oDet.dFaltas,
                                                                                                                                "@Devolucion", oDet.dDevolucion,
                                                                                                                                "@DevolucionCreditoInfo", oDet.dDevolucionCredInfo,
                                                                                                                                "@IncidenciasQ3MexJet", oDet.dIncidenciasQ3MexJet,
                                                                                                                                "@PagoIncapacidad", oDet.dPagoIncapacidad,
                                                                                                                                "@Compensacion", oDet.dCompensacion,
                                                                                                                                "@CompensacionRetro", oDet.dCompensacionPorRetro,
				                                                                                                                "@CompensacionFija", oDet.dCompensacionFija,
                                                                                                                                "@DevolucionPorFaltas", oDet.dDevolucionPorFaltas,
                                                                                                                                "@DevolucionPorPrestamo", oDet.dDevolucionPorPrestamo,
                                                                                                                                "@Aguinaldo", oDet.dAguinaldo,
                                                                                                                                "@DiasLaborados", oDet.dDiasLaborados,
                                                                                                                                "@SD", oDet.dSD,
                                                                                                                                "@PagoPropina", oDet.dPagoPropina,
				                                                                                                                "@Retroactivo", oDet.dRetroactivo,
				                                                                                                                "@HorasDiasFestivos", oDet.dHorasDiasFestivos,
				                                                                                                                "@HorasDiasFestivosTLC", oDet.dHorasFestivosTLC,
				                                                                                                                "@HorasDiasFestivosADN", oDet.dHorasFestivosADN,
				                                                                                                                "@PrimaDominical", oDet.dPrimaDominical,
				                                                                                                                "@PrimaDominicalTLC", oDet.dPrimaDominicalTLC,
				                                                                                                                "@PrimaDominicalADN", oDet.dPrimaDominicalADN,
                                                                                                                                "@PrimaVacacionalTLC", oDet.dPrimaVacacionalTLC,
                                                                                                                                "@PrimaVacacionalADN", oDet.dPrimaVacacionalADN,
				                                                                                                                "@PrimaVacacional", oDet.dPrimaVacacional,
                                                                                                                                "@INCXAntiguedad", oDet.dINCXAntiguedad,
				                                                                                                                "@PrimaAntiguedad", oDet.dPrimaAntiguedad,
				                                                                                                                "@Bono", oDet.dBono,
				                                                                                                                "@TotalIngresos", oDet.dTotalIngresos,
				                                                                                                                "@SalarioDiario", oDet.dSalarioDiario,
				                                                                                                                "@SalarioIntegrado", oDet.dSalarioIntegrado,
				                                                                                                                "@Sueldo", oDet.dSueldo,
				                                                                                                                "@SeptimoDia", oDet.dSeptimoDia,
				                                                                                                                "@HorasExtras2", oDet.dHorasExtras2,
				                                                                                                                "@Destajos", oDet.dDestajos,
				                                                                                                                "@PremioEficiencia", oDet.dPremioEficiencia,
				                                                                                                                "@Vacaciones", oDet.dVacaciones,
				                                                                                                                "@PrimaVacacional2", oDet.dPrimaVacacional2,
				                                                                                                                "@Aguinaldo2", oDet.dAguinaldo2,
				                                                                                                                "@OtrasPercepciones", oDet.dOtrasPercepciones,
				                                                                                                                "@TotalPercepciones", oDet.dTotalPercepciones,
				                                                                                                                "@RetInvVida", oDet.dRetInvVida,
				                                                                                                                "@RetCesantia", oDet.dRetCesantia,
				                                                                                                                "@RetEnfMatObrero", oDet.dRetEnfMatObrero,
				                                                                                                                "@SeguroViviendaInfonavit", oDet.dSeguroViviendaInfonavit,
				                                                                                                                "@SubsEmpleoAcreditado", oDet.dSubsEmpleoAcreditado,
				                                                                                                                "@SubsidioEmpleo", oDet.dSubsidioEmpleo,
				                                                                                                                "@ISRAntesSubsEmpleo", oDet.dISRAntesSubsEmpleo,
                                                                                                                                "@ISR_Art142", oDet.dISR_Art142,
				                                                                                                                "@ISR_SP", oDet.dISR_SP,
				                                                                                                                "@IMSS", oDet.dIMSS,
				                                                                                                                "@PrestamoInfonavit", oDet.dPrestamoInfonavit,
                                                                                                                                "@PrestamoEmpresa", oDet.dPrestamoEmpresa,
				                                                                                                                "@AjusteNeto", oDet.dAjusteNeto,
				                                                                                                                "@PensionAlimenticia", oDet.dPensionAlimenticia,
				                                                                                                                "@OtrasDeducciones", oDet.dOtrasDeducciones,
				                                                                                                                "@TotalDeducciones", oDet.dTotalDeducciones,
				                                                                                                                "@Neto", oDet.dNeto,
                                                                                                                                "@NetoIMSSReal", oDet.dNetoIMSSReal,
				                                                                                                                "@ValesGratificacion", oDet.dValesGratificacion,
				                                                                                                                "@InvalidezVida", oDet.dInvalidezVida,
				                                                                                                                "@CesantiaVejez", oDet.dCesantiaVejez,
				                                                                                                                "@EnfMatPatron", oDet.dEnfMatPatron,
				                                                                                                                "@FondoRetiroSAR2Porciento", oDet.dFondoRetiroSAR2Porciento,
                                                                                                                                "@ImpuestoEstatal3Porciento", oDet.dImpuestoEstatal3Porciento,
				                                                                                                                "@RiesgoTrabajo", oDet.dRiesgoTrabajo,
				                                                                                                                "@IMSSEmpresa", oDet.dIMSSEmpresa,
				                                                                                                                "@InfonavitEmpresa", oDet.dInfonavitEmpresa,
				                                                                                                                "@GuarderiaIMSS", oDet.dGuarderiaIMSS,
				                                                                                                                "@OtrasObligaciones", oDet.dOtrasObligaciones,
				                                                                                                                "@TotalObligaciones", oDet.dTotalObligaciones,
				                                                                                                                "@EmpresaPagaAsimilados", oDet.dEmpresaPagaAsimilados,
				                                                                                                                "@Asimilados", oDet.dAsimilados,
				                                                                                                                "@ISR", oDet.dISR,
				                                                                                                                "@TotalPagarAsimiladosSalario", oDet.dTotalPagarAsimiladosSalario,
				                                                                                                                "@PrestamoCompania", oDet.dPrestamoCompania,
				                                                                                                                "@InteresesPrestamo", oDet.dInteresesPrestamo,
				                                                                                                                "@RecuperacionAsesores", oDet.dRecuperacionAsesores,
                                                                                                                                "@GastosNoComprobados", oDet.dGastosNoComprobados,
				                                                                                                                "@DescuentoGMM", oDet.dDescuentoGMM,
                                                                                                                                "@CursoIngles", oDet.dCursoIngles,
                                                                                                                                "@DescuentoSportium", oDet.dDescuentoSportium,
                                                                                                                                "@DescuentoPorMerma", oDet.dDescuentoPorMerma,
				                                                                                                                "@DescuentoPorPagoMas", oDet.dDescuentoPorPagoDeMas,
				                                                                                                                "@DescuentoPorFaltas", oDet.dDescuentoPorFaltas,
				                                                                                                                "@GastosNoComprobados2", oDet.dGastosNoComprobados2,
				                                                                                                                "@Ayudante", oDet.dAyudante,
				                                                                                                                "@DescuentoClienteOtros", oDet.dDescuentoClienteOtros,
				                                                                                                                "@Otros", oDet.dOtros,
				                                                                                                                "@PensionAlimenticia2", oDet.dPensionAlimenticia2,
				                                                                                                                "@NetoPagarAsimilados", oDet.dNetoPagarAsimilados,
                                                                                                                                "@TarjetaEmpresarialISR", oDet.dTarjetaEmpresarialISR,
				                                                                                                                "@SueldoIMSS", oDet.dSueldoIMSS,
				                                                                                                                "@AsimiladosSalarios", oDet.dAsimiladosSalarios,
                                                                                                                                "@TarjetaEmpresarialISR2", oDet.dTarjetaEmpresarialISR2,
				                                                                                                                "@CargaPatronal", oDet.dCargaPatronal,
				                                                                                                                "@MARKUP7_5Porciento", oDet.dMARKUP7_5Porciento,
				                                                                                                                "@FactorChequeCertSub", oDet.dFactorChequeCertSub,
				                                                                                                                "@CostoEmpleado", oDet.dCostoEmpleado,
				                                                                                                                "@Importe", oDet.dImporte,
				                                                                                                                "@IVA", oDet.d_IVA,
				                                                                                                                "@TotalFactura", oDet.dTotalFactura
                                                                                                                         );
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DataTable DBGetObtieneExistenciaRFC(string sRFC)
        {
            try
            {
                string sResult = "SELECT COUNT(1) RFC FROM [Aerolineas_Ejecutivas].[dbo].[ALE_RH] WHERE RFC = '" + sRFC + "'";
                return new DBBaseSAP().oDB_SP.EjecutarDT_DeQuery(sResult);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}