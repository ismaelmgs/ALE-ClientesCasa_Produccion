﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="frmCorteGastos.aspx.cs" EnableEventValidation="false" UICulture="es" Culture="es-MX" Inherits="ClientesCasa.Views.Catalogos.frmCorteGastos" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        $("[src*=read_more]").live("click", function () {
            $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
            $(this).attr("src", "../../Images/icons/not_more.png");
        });
        $("[src*=not_more]").live("click", function () {
            $(this).attr("src", "../../Images/icons/read_more.png");
            $(this).closest("tr").next().remove();
        });

    </script>

    <script type="text/javascript">
        $(document).ready(function () {
            gridviewScroll();
        });

        function On(GridView) {
            if (GridView != null) {
                GridView.originalBgColor = GridView.style.backgroundColor;
                GridView.style.backgroundColor = "#C0BCBC";
            }
        }

        function Off(GridView) {
            if (GridView != null) {
                GridView.style.backgroundColor = GridView.originalBgColor;
            }
        }

        function UserDeleteConfirmation() {
            if (confirm("¿Realmente esta seguro de eliminar el gasto?"))
                return true;
            else
                return false;
        }

        

        function EnterEvent(e) {

            if (e.keyCode == 13) {
                var obj = document.getElementById('<%=btnBuscarCliente.ClientID%>');

                if (obj) {
                    obj.click();
                }
            }
        }

        function visible(element) {
            var elemento = document.getElementById(element);
            elemento.style.display = 'block';
        }

        function invisible(element) {
            var elemento = document.getElementById(element);
            elemento.style.display = 'none';
        }

        function Selrdbtn(id) {
            var rdBtn = document.getElementById(id);
            var List = document.getElementsByTagName("input");
            for (i = 0; i < List.length; i++) {
                if (List[i].type == "radio" && List[i].id != rdBtn.id) {
                    List[i].checked = false;
                }
            }
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            return true;
        }

        function OcultarModalPeriodo() {

            var modalId = '<%=mpePeriodo.ClientID%>';
            var modal = $find(modalId);
            modal.hide();
        }
        
        function OcultarModalConfir() {

            var modalId = '<%=mpeConfirm.ClientID%>';
            var modal = $find(modalId);
            modal.hide();
        }

        

        
    </script>

    <style type="text/css">
        .AlineadoDerecha{
            text-align:right;
            }
    </style>


    <cc1:ToolkitScriptManager ID="ToolKitScriptManager" runat="server"></cc1:ToolkitScriptManager>
    <asp:UpdatePanel ID="upaPrincipal" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlBusqueda" runat="server">
                <div style="text-align: left">
                    <h4>&nbsp;&nbsp;Corte Mensual de Gastos</h4>
                </div>
                <div class="widget-box">
                    <div class="widget-title">
                        <span class="icon"><i class="icon-align-justify"></i></span>
                        <h5>Búsqueda de Clientes</h5>
                    </div>
                    <div class="widget-content nopadding">

                        <div class="control-group">
                            <div class="controls">
                                <br />
                                <div class="table-responsive" style="margin: 5px;">
                                    <table style="width: 100%" class="table table-striped">
                                        <tr>
                                            <td style="text-align: center">
                                                <div class="section group" style="margin-left: -30px;">
                                                    <div class="col span_1_of_4">
                                                        &nbsp; Búsqueda:
                                                    </div>
                                                    <div class="col span_1_of_4">
                                                        <asp:TextBox ID="txtBusqueda" runat="server" MaxLength="98" Width="90%"
                                                            ClientIDMode="Static" onkeypress="return EnterEvent(event);"></asp:TextBox>
                                                    </div>
                                                    <div class="col span_1_of_4">
                                                        <asp:DropDownList ID="ddlOpcBus" runat="server" placeholder="Seleccione">
                                                            <asp:ListItem Text="Clave Cliente" Value="2"></asp:ListItem>
                                                            <asp:ListItem Text="Clave Contrato" Value="3"></asp:ListItem>
                                                            <asp:ListItem Text="Matrícula" Value="4" Selected="True"></asp:ListItem>
                                                            <asp:ListItem Text="Solo Activos" Value="1"></asp:ListItem>
                                                            <asp:ListItem Text="Inactivos" Value="0"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col span_1_of_4">
                                                        <asp:Button ID="btnBuscarCliente" runat="server" Text="Buscar" OnClick="btnBuscarCliente_Click" ClientIDMode="Static" CssClass="btn btn-success" />
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <table style="width: 100%">
                            <tr>
                                <td>
                                    <div class="table-responsive" style="margin: 5px;">
                                        <asp:GridView ID="gvClientes" runat="server" AutoGenerateColumns="False" DataKeyNames="IdContrato"
                                            AllowPaging="True" Width="100%" CssClass="table table-bordered table-striped table-hover"
                                            PageSize="5" OnRowDataBound="gvClientes_RowDataBound" OnPageIndexChanging="gvClientes_PageIndexChanging" OnSelectedIndexChanged="gvClientes_SelectedIndexChanged">
                                            <EmptyDataTemplate>
                                                No existen Registros para mostrar.
                                            </EmptyDataTemplate>
                                            <Columns>
                                                <asp:BoundField DataField="ClaveCliente" HeaderText="Clave del Cliente" />
                                                <asp:BoundField DataField="Nombre" HeaderText="Nombre del cliente" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="ClaveContrato" HeaderText="Clave del contrato" />
                                                <asp:BoundField DataField="Matricula" HeaderText="Matrícula Aeronave" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </td>
                            </tr>
                        </table>

                        <br />
                        <br />
                    </div>
                </div>
            </asp:Panel>

            <br />

            <asp:Panel ID="pnlCorte" runat="server" Visible="false">
                <asp:UpdatePanel ID="upaCorte" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table style="width: 100% !important;">
                            <tr>
                                <td style="text-align: center !important;">
                            
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: center !important;">
                                    <br />
                                    <strong>
                                        <asp:Label ID="lblMex" runat="server" Text="Mes:" Font-Bold="true"></asp:Label></strong>
                                    <asp:Label ID="lblReqMes" runat="server" Text="" Font-Bold="true"></asp:Label>
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <strong>
                                        <asp:Label ID="lblAnio" runat="server" Text="Año:" Font-Bold="true"></asp:Label></strong>
                                    <asp:Label ID="lblReqAnio" runat="server" Text="" Font-Bold="true"></asp:Label>
                                </td>
                            </tr>
                        </table>

                        <div style="text-align: center; font-size:13px;">
                            <strong>
                                <asp:Label ID="lblClaveCliente" runat="server"></asp:Label></strong>&nbsp;&nbsp;&nbsp;&nbsp;
                            <strong>
                                <asp:Label ID="lblNombreCliente" runat="server"></asp:Label></strong>&nbsp;&nbsp;&nbsp;&nbsp;
                            <strong>
                                <asp:Label ID="lblMatricula" runat="server"></asp:Label></strong>
                        </div>
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-align-justify"></i></span>
                                <h5>Info.</h5>
                            </div>
                            <div class="widget-content nopadding">
                                <br />
                                <div class="control-group">
                                    <div class="controls">
                                        <center>
                                            <div class="table-responsive" style="width: 80%; border:1px solid #dddddd;">
                                                <table class="table">
                                                    <tr>
                                                        <td>
                                                            <div class="table-responsive" style="margin: 5px;">
                                                                <asp:GridView ID="gvCorteMensual" runat="server" AutoGenerateColumns="False"
                                                                    AllowPaging="True" Width="100%" CssClass="table table-bordered table-striped table-hover">
                                                                    <EmptyDataTemplate>
                                                                        No existen Registros para mostrar.
                                                                    </EmptyDataTemplate>
                                                                    <Columns>
                                                                        <asp:BoundField DataField="TipoMoneda" HeaderText="Moneda" />
                                                                        
                                                                        <asp:TemplateField HeaderText="Saldo anterior" ItemStyle-HorizontalAlign="Right">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblImporteOriginalUSD" Text='<%# Bind("SaldoAnterior", "{0:c}") %>' runat="server" Style=" display:block; text-align: right;"></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Pagos crédito" ItemStyle-HorizontalAlign="Right">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblImporteOriginalUSD" Text='<%# Bind("PagosCreditos", "{0:c}") %>' runat="server" Style=" display:block; text-align: right;"></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Nuevos cargos" ItemStyle-HorizontalAlign="Right">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblImporteOriginalUSD" Text='<%# Bind("NuevosCargos", "{0:c}") %>' runat="server" Style=" display:block; text-align: right;"></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Saldo actual" ItemStyle-HorizontalAlign="Right">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblImporteOriginalUSD" Text='<%# Bind("SaldoActual", "{0:c}") %>' runat="server" Style=" display:block; text-align: right;"></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div style="text-align: right; margin-right: 15px;">
                                            <br />
                                                <asp:Button ID="btnCorte" runat="server" Text="Generar Corte" CssClass="btn btn-primary"
                                                    OnClick="btnCorte_Click"/>
                                            </div>
                                        </center>
                                        <div>
                                            <asp:Label ID="lblValidacion" runat="server" Text="" ForeColor="Red"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </ContentTemplate>
                    <Triggers>
                        
                    </Triggers>
                </asp:UpdatePanel>
            </asp:Panel>

        </ContentTemplate>
    </asp:UpdatePanel>

    
    <%-- Modal de Periodo --%>
    <asp:HiddenField ID="hdTargetPeriodo" runat="server" />
    <cc1:ModalPopupExtender ID="mpePeriodo" runat="server" TargetControlID="hdTargetPeriodo"
        PopupControlID="pnlPeriodo" BackgroundCssClass="overlayy">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlPeriodo" runat="server" BorderColor="" BackColor="White" Height="150px"
        Width="280px" HorizontalAlign="Center" Style="display: none" CssClass="modalrlr">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table style="width: 100%">
                    <tr>
                        <td colspan="2">
                            <h4>
                                <asp:Label ID="Label2" runat="server" Text="Seleccione el periodo"></asp:Label></h4>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 30%">
                            <asp:Label ID="Label1" runat="server" Text="Periodo:" CssClass="lblInput"></asp:Label>
                        </td>
                        <td style="width: 70%">
                            <asp:TextBox ID="txtPeriodo" type="date" runat="server" placeholder="MM/YYYY" CssClass="form-control"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%">
                    <tr>
                        <td style="width: 50%">
                            <div style="text-align: right; float: right">
                                <asp:Button ID="btnAceptarPeriodo" runat="server" Text="Aceptar" OnClientClick="OcultarModalPeriodo();" OnClick="btnAceptarPeriodo_Click" CssClass="btn btn-primary" />
                            </div>
                        </td>
                        <td style="width: 50%">
                            <div style="text-align: left; float: left">
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClientClick="OcultarModalPeriodo();" CssClass="btn btn-default" />
                            </div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    
    <%-- Modal Confirm --%>
    <asp:HiddenField ID="hdTargetConfirm" runat="server" />
    <cc1:ModalPopupExtender ID="mpeConfirm" runat="server" TargetControlID="hdTargetConfirm"
        PopupControlID="pnlConfirm" BackgroundCssClass="overlayy">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlConfirm" runat="server" BackColor="White" Style="display: none;" CssClass="modalrlr">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <table style="width: 100%">
                    <tr>
                        <td colspan="2" runat="server" id="tdCaption">&nbsp;
                            <center>
                                <h4><asp:Label ID="lblCaption" runat="server"></asp:Label></h4>
                            </center>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 60px; vertical-align: middle; text-align: center">
                            <asp:Image ID="imgInfo" runat="server" ImageUrl="~/Images/icons/information.png" Height="24" Width="24" />
                        </td>
                        <td style="text-align: left; vertical-align: middle">
                            <asp:Label ID="lblMessageConfirm" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="text-align: right">
                            <asp:Button ID="btnAceptConfirm" runat="server" Text="Si" OnClientClick="OcultarModalConfir();" OnClick="btnAceptConfirm_Click" CssClass="btn btn-primary" />
                            <asp:Button ID="btnCancelConfirm" runat="server" Text="No" OnClick="btnCancelConfirm_Click" CssClass="btn btn-default" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>

    <%--<asp:UpdateProgress ID="prgLoadingStatus" runat="server" DynamicLayout="true">
        <ProgressTemplate>
            <div id="overlay">
                <div id="modalprogress">
                    <div id="theprogress">
                        <asp:Image ID="imgWaitIcon" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/icons/loading-animated.gif" Width="150" Height="130" />
                    </div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>

</asp:Content>
