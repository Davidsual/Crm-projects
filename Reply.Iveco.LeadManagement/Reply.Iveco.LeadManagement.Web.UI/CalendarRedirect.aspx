<%@ Page Language="C#" %>
<script runat="server">
    void Page_Load(object sender, EventArgs e)
    {
        string url = "../Custom" + Request.QueryString["orgname"].ToUpper()  + "/ContainerAdministratorScheduler.aspx?" + Request.QueryString.ToString();
        Response.Redirect(url);
    }
</script>




