# AzureManagedIdentityPOC
Sample Asp.net application to connect to Azure SQL using Managed Identity 

Follow the below steps to enable Manged Identity connection to App Service:
1. Grant database access to Azure AD user: Enable Azure AD authentication to SQL Database by assigning an Azure AD user as the Active Directory admin of the server.
   <br/> <pre>azureaduser=$(az ad user list --filter "userPrincipalName eq '&lt;user-principal-name&gt;'" --query [].objectId --output tsv)
    az sql server ad-admin create --resource-group myResourceGroup --server-name &lt;server-name&gt; --display-name ADMIN --object-id $azureaduser
    </pre>
    
2. Set up Visual Studio
    1. Add your Azure AD user in Visual Studio by selecting File > Account Settings from the menu, and click Add an account.
    2. To set the Azure AD user for Azure service authentication, select Tools > Options from the menu, then select Azure Service Authentication > Account Selection. Select the Azure AD user you added and click OK.
3. Change Code
    1. Add the NuGet package
        Install-Package Microsoft.Azure.Services.AppAuthentication -Version 1.4.0
      
    2. Change Web.config
    <br/>&nbsp;&nbsp;&nbsp;1. Add the following section in "configSections"
      <pre>  &lt;section name="SqlAuthenticationProviders" type="System.Data.SqlClient.SqlAuthenticationProviderConfigurationSection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" /&gt; </pre>
      
     <br/>&nbsp;&nbsp;&nbsp;2. After ConfigSection Add
       <pre>  &lt;SqlAuthenticationProviders&gt;
  &lt;providers&gt;
    <add name="Active Directory Interactive" type="Microsoft.Azure.Services.AppAuthentication.SqlAppAuthenticationProvider, Microsoft.Azure.Services.AppAuthentication" /&gt;
  &lt;/providers&gt;
&lt;/SqlAuthenticationProviders&gt;</pre>

    <br/>&nbsp;&nbsp;&nbsp;3. Change Connectionstring to 
    <br/> <pre>"server=tcp:&lt;server-name&gt;.database.windows.net;database=&lt;db-name&gt;;UID=AnyString;Authentication=Active Directory Interactive"
  </pre>
  
4. Enable managed identity on app
     <pre>  az webapp identity assign --resource-group myResourceGroup --name &lt;app-name&gt;  </pre>
  
5. Grant permissions to managed identity
    1. Sign in to SQL Database by using the SQLCMD command
           <pre> sqlcmd -S <server-name>.database.windows.net -d <db-name> -U &lt;aad-user-name&gt; -P "&lt;aad-password&gt;" -G -l 30 </pre>
    2. Grant the permissions your app in SQL
    <pre>
            CREATE USER [<identity-name>] FROM EXTERNAL PROVIDER;
            ALTER ROLE db_datareader ADD MEMBER [<identity-name>];
            ALTER ROLE db_datawriter ADD MEMBER [<identity-name>];
            ALTER ROLE db_ddladmin ADD MEMBER [<identity-name>];
            GO
            </pre>
6. Modify connection string in App Service
     <pre>  az webapp config connection-string delete --resource-group myResourceGroup --name &lt;app-name&gt; --setting-names MyDbConnection </pre>
