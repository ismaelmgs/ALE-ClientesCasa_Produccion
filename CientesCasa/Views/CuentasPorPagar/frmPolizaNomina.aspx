<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="frmPolizaNomina.aspx.cs" Inherits="ClientesCasa.Views.ASC.frmPolizaNomina" 
    UICulture="es" Culture="es-MX" UnobtrusiveValidationMode="None" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .validar {
            color:#ff0000;
            font-family:'Arial Rounded MT';
            font-size:10pt;
        }
    </style>

    <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
    <div class="content">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-12">
                    <div class="card" style="height: 1000px;">

                        <div class="card-header" data-background-color="blue">
                            <h4 class="title">Carga de Póliza de Nómina</h4>
                        </div>

                        <div class="card-content table-responsive">
                            <fieldset>
                                <legend><p class="category">Seleccione el periodo del archivo.</p></legend>
                                <%--<div class="col-md-12">--%>

                                <div class="row">
                                    <div class="col-md-12">
                                        <div style="width:100%; padding-bottom:20px;" align="center">
                                            <table border="0" class="table-responsive" width="60%">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblFechaInicio" runat="server" Text="Fecha inicial"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtFechaInicio" type="date" runat="server" placeholder="dd/mm/aaaa" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                                        <label for="txtFechaInicio" style="height:24px; width:24px" class="input-group-addon generic_btn"></label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblFechaFinal" runat="server" Text="Fecha final"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtFechaFinal" type="date" runat="server" placeholder="dd/mm/aaaa" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                                        <label for="txtFechaFinal" style="height:24px; width:24px" class="input-group-addon generic_btn"></label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" align="center">
                                                        <asp:RequiredFieldValidator ID="rqFechaIni" runat="server" ErrorMessage="*Ingresa la Fecha de Inicio"
                                                            ControlToValidate="txtFechaInicio" Display="Dynamic" ValidationGroup="groupFechas" CssClass="validar">
                                                        </asp:RequiredFieldValidator>
                                                    </td>
                                                    <td colspan="2" align="center">
                                                        <asp:RequiredFieldValidator ID="rqFechaFin" runat="server" ErrorMessage="*Ingresa la Fecha Final"
                                                            ControlToValidate="txtFechaFinal" Display="Dynamic" ValidationGroup="groupFechas" CssClass="validar">
                                                        </asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                                    
                                <%--</div>--%>
                                        
                                <div class="col-md-12">
                                    <div style="width:100%;" align="center">
                                        <asp:Label ID="lblSelect" runat="server" Text="" Font-Size="10pt" Font-Bold="true" Visible="false"></asp:Label>
                                        <asp:FileUpload ID="fluArchivo" runat="server" CssClass="btn btn-success" style="width:325px !important;" />
                                        <asp:Button ID="btnLeer" runat="server" Text="Validar" OnClick="btnLeer_Click" CssClass="btn btn-info" ValidationGroup="groupFechas" />
                                    </div>
                                </div>
                            </fieldset>
                        </div>

                        <br />
                        <br />

                        <asp:UpdatePanel ID="upaCarga" runat="server">
                            <ContentTemplate>

                                <div class="alert alert-success" role="alert" id="msgSuccesss" runat="server" style="width:95%; height:auto;" visible="false">
                                    <strong>Correcto!&nbsp;</strong> <asp:Label ID="lblSuccess" runat="server" Text="" Font-Size="9pt" ForeColor="Green"></asp:Label>
                                </div>
                                <div class="alert alert-danger" role="alert" id="msgError" runat="server"  style="width:95%; height:auto;" visible="false">
                                    <strong>Error!&nbsp;</strong><asp:Label ID="lblError" runat="server" Text="" Font-Size="9pt" ForeColor="Red"></asp:Label>
                                </div>

                                <div class="card-content table-responsive">
                                    <fieldset>
                                        <div class="col-md-2">
                                        </div>
                                        <div class="col-md-8">
                                            <div align="left" style="overflow-y:auto; max-height:400px; overflow-x:auto;">
                                                <asp:GridView ID="gvDatos" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-striped table-hover">
                                                   <Columns>
                                                       <asp:BoundField DataField="Empresa" HeaderText="Empresa" HeaderStyle-CssClass="gvCenter" ItemStyle-CssClass="gvLeft" />
                                                       <asp:BoundField DataField="Periodo" HeaderText="Periodo" HeaderStyle-CssClass="gvCenter" ItemStyle-CssClass="gvLeft" />
                                                       <asp:BoundField DataField="TipoMovimiento" HeaderText="TipoMovimiento" HeaderStyle-CssClass="gvCenter" ItemStyle-CssClass="gvLeft" />
                                                       <asp:BoundField DataField="Factura" HeaderText="Factura" HeaderStyle-CssClass="gvCenter" ItemStyle-CssClass="gvLeft" />
                                                       <asp:BoundField DataField="TipoNomina" HeaderText="TipoNomina" HeaderStyle-CssClass="gvCenter" ItemStyle-CssClass="gvLeft" />
                                                       <asp:BoundField DataField="Nombre" HeaderText="Nombre" HeaderStyle-CssClass="gvCenter" ItemStyle-CssClass="gvLeft" />
                                                       <asp:BoundField DataField="RFC" HeaderText="RFC" HeaderStyle-CssClass="gvCenter" ItemStyle-CssClass="gvLeft" />
                                                       <asp:BoundField DataField="SalarioMensual" DataFormatString="{0:C}" HeaderText="Salario Mensual" HeaderStyle-CssClass="gvCenter" ItemStyle-CssClass="gvRight" />
                                                   </Columns>
                                                    <HeaderStyle CssClass="gvHeader" />
                                                    <AlternatingRowStyle CssClass="gvAlternate" />
                                                    <RowStyle CssClass="gvItemsRows" />
                                                    <FooterStyle CssClass="gvFooter" />
                                                    <PagerStyle CssClass="gvFooter" />
                                                    <EmptyDataTemplate>
                                                        No existen registros para mostrar.
                                                    </EmptyDataTemplate>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                        <div class="col-md-2">
                                        </div>

                                    </fieldset>
                                </div>

                                <asp:Panel ID="pnlSimulacion" runat="server" Visible="false">
                                    <div class="col-md-12">
                                    <fieldset>
                                        <legend><h3><asp:Label ID="lblTituloProceso" runat="server" Text=""></asp:Label></h3></legend>
                                        <div align="center" style="overflow-y:auto; max-height:450px;">
                                            <table width="100%" class="table table-striped">
                                                <tr>
                                                    <td>
                                                        <asp:GridView ID="gvResultado" AutoGenerateColumns="false" runat="server" Width="100%" 
                                                            CssClass="table table-bordered table-striped table-hover">
                                                            <Columns>
                                                                <asp:BoundField DataField="Fila" HeaderText="FILA" ItemStyle-Width="10%" HeaderStyle-CssClass="gvCenter" ItemStyle-CssClass="gvCenter" />
                                                                <asp:BoundField DataField="Campo" HeaderText="CAMPO" ItemStyle-Width="20%" HeaderStyle-CssClass="gvCenter" ItemStyle-CssClass="gvCenter" />  
                                                                <asp:BoundField DataField="Valor" HeaderText="VALOR" ItemStyle-Width="20%" HeaderStyle-CssClass="gvCenter" ItemStyle-CssClass="gvCenter" />  
                                                                <asp:BoundField DataField="Status" HeaderText="STATUS" ItemStyle-Width="10%" HeaderStyle-CssClass="gvCenter" ItemStyle-CssClass="gvCenter" />  
                                                                <asp:BoundField DataField="Descripcion" HeaderText="DESCRIPCIÓN" ItemStyle-Width="40%" HeaderStyle-CssClass="gvCenter" />
                                                            </Columns>
                                                            <HeaderStyle CssClass="gvHeader" />
                                                            <AlternatingRowStyle CssClass="gvAlternate" />
                                                            <RowStyle CssClass="gvItemsRows" />
                                                            <FooterStyle CssClass="gvFooter" />
                                                            <PagerStyle CssClass="gvFooter" />
                                                            <EmptyDataTemplate>
                                                                No hay registros.
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </fieldset>
                                    </div>
                                </asp:Panel>
                        
                                <div style="width:90%; text-align:right">
                                    <asp:Panel ID="pnlBotonesProcesar" runat="server" Visible="false">
                                        <asp:HiddenField ID="hdnValidacion" runat="server" />
                                        <asp:Button ID="btnInsertar" runat="server" Text="Insertar" Enabled="false" OnClientClick="return confirm('¿Realmente desea insertar la información?');" 
                                            CssClass="btn btn-primary" OnClick="btnInsertar_Click" />
                                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary"/>
                                    </asp:Panel>
                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>

                </div>

               

                <div class="col-md-6">
                </div>
                <div class="col-md-6">
                    
                </div>
            </div>
        </div>
    </div>

     <asp:UpdateProgress ID="prgLoadingStatus" runat="server" DynamicLayout="true" AssociatedUpdatePanelID="upaCarga">
        <ProgressTemplate>
            <div id="overlay">
                <div id="modalprogress">
                    <div id="theprogress">
                        <asp:Image ID="imgWaitIcon" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/icons/loading-animated.gif" Width="150" Height="130" />
                    </div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>
