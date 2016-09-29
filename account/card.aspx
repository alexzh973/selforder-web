<%@ Page Title="" Language="C#" MasterPageFile="~/COMMON/Preview.Master"
    AutoEventWireup="true" CodeBehind="card.aspx.cs" Inherits="wstcp.personcard" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    

    <div class="f-bwi">
        <asp:Literal ID="photo" runat="server"></asp:Literal>

        <div class="f-bwi-text">
            <h2>
                <asp:Literal ID="lbName" runat="server"></asp:Literal></h2>
            <p>
                <asp:Literal ID="phones" runat="server"></asp:Literal>
            </p>
            <p>
                <asp:Literal ID="email" runat="server"></asp:Literal>
            </p>

            <asp:PlaceHolder ID="blockOwner" runat="server">

                <p>
                    <span class="bold bigsize">
                        <asp:Literal ID="lbOwnerName" runat="server"></asp:Literal></span>
                </p>
                <p>
                    <asp:Literal ID="lbOwnerAddress" runat="server"></asp:Literal>
                </p>
                <p>
                    <asp:Literal ID="lbOwnerPhones" runat="server"></asp:Literal>
                </p>
                <p>
                    <asp:Literal ID="lbOwnerTime" runat="server"></asp:Literal>
                </p>


            </asp:PlaceHolder>
            <asp:PlaceHolder ID="blockSubject" runat="server">
                <asp:Literal ID="lbSubjectInfo" runat="server"></asp:Literal>

                <h4>Закрепленный менеджер Сантехкомплект:</h4>
                <div class="f-bwi">
                    <asp:Image ID="imgPhotoTA" CssClass="f-bwi-pic" runat="server" />
                    <div class="f-bwi-text">
                        <p><asp:Literal ID="lbTAName" runat="server"></asp:Literal></p>
                        <p>тел. <asp:Literal ID="lbTAPhone" runat="server"></asp:Literal></p>
                        <p>email <asp:Literal ID="lbTAEmail" runat="server"></asp:Literal></p>
                        <br/>
                        <p>
                            <asp:Label ID="lbTaOwnName" CssClass="bigsize bold" runat="server" Text=""></asp:Label></p>
                        <p>
                            <asp:Label ID="lbTaOwnAddress" runat="server" Text=""></asp:Label></p>
                        <p>
                            <asp:Label ID="lbTaOwnPhones" runat="server" Text=""></asp:Label></p>
                        <p>
                            <asp:Label ID="lbTaOwnTime" runat="server" Text=""></asp:Label></p>
                    </div><!-- f-bwi-text -->

                </div><!-- f-bwi -->


            </asp:PlaceHolder>
            

        </div>
        
    </div>
    
    <asp:Panel ID="pnlFormMessage" runat="server">
        <div class="border shadow">
            <div class="g-row">

                <h4>Сообщение</h4>
                <div class="message">
                    <asp:Literal ID="lbMess" runat="server"></asp:Literal>
                </div>
                <div>
                    <strong>Получатели сообщения</strong>
                    <div>
                        <asp:CheckBox ID="chTosupport" ClientIDMode="Static" runat="server" Text="Служба поддержки" />
                        <label >
                            <asp:Literal
                                ID="lbsupportemail" runat="server"></asp:Literal></label>
                    </div>
                    <div>
                        <asp:CheckBox ID="chToTa" ClientIDMode="Static" runat="server" Text="email персонального менеджера" />
                        <label >
                            <asp:Literal ID="lbToTaEmail" runat="server"></asp:Literal></label>
                    </div>

                </div>


                <div>
                    <asp:TextBox ID="txMessage" runat="server" MaxLength="500" TextMode="MultiLine" Width="100%"></asp:TextBox>
                </div>

            </div>

            <div class="g-row">
                <asp:Button ID="btnSend" runat="server" CssClass="f-bu-default" Text="Отправить" OnClick="btnSend_Click" />
            </div>
        </div>


    </asp:Panel>
</asp:Content>
