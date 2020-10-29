# AzureManagedIdentityPOC
Sample Asp.net application to connect to Azure SQL using Managed Identity 


# 
1. Grant database access to Azure AD user: Enable Azure AD authentication to SQL Database by assigning an Azure AD user as the Active Directory admin of the server.
    azureaduser=$(az ad user list --filter "userPrincipalName eq '<user-principal-name>'" --query [].objectId --output tsv)
    az sql server ad-admin create --resource-group myResourceGroup --server-name <server-name> --display-name ADMIN --object-id $azureaduser

2. Set up Visual Studio
  a) Add your Azure AD user in Visual Studio by selecting File > Account Settings from the menu, and click Add an account.
  b) To set the Azure AD user for Azure service authentication, select Tools > Options from the menu, then select Azure Service Authentication > Account Selection. Select the Azure AD user you added and click OK.
  
3. Change Code
  a) Add the NuGet package
      Install-Package Microsoft.Azure.Services.AppAuthentication -Version 1.4.0
      
  b) Change Web.config
      i) Add the following section in <configSections>
        <section name="SqlAuthenticationProviders" type="System.Data.SqlClient.SqlAuthenticationProviderConfigurationSection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
      ii) After ConfigSection Add
        <SqlAuthenticationProviders>
  <providers>
    <add name="Active Directory Interactive" type="Microsoft.Azure.Services.AppAuthentication.SqlAppAuthenticationProvider, Microsoft.Azure.Services.AppAuthentication" />
  </providers>
</SqlAuthenticationProviders>
      iii) Change Connectionstring to "server=tcp:<server-name>.database.windows.net;database=<db-name>;UID=AnyString;Authentication=Active Directory Interactive"
  
  4. Enable managed identity on app
      az webapp identity assign --resource-group myResourceGroup --name <app-name>
  
  5. Grant permissions to managed identity
       a) sign in to SQL Database by using the SQLCMD command
            sqlcmd -S <server-name>.database.windows.net -d <db-name> -U <aad-user-name> -P "<aad-password>" -G -l 30
       b) Grant the permissions your app in SQL
            CREATE USER [<identity-name>] FROM EXTERNAL PROVIDER;
            ALTER ROLE db_datareader ADD MEMBER [<identity-name>];
            ALTER ROLE db_datawriter ADD MEMBER [<identity-name>];
            ALTER ROLE db_ddladmin ADD MEMBER [<identity-name>];
            GO
  
  6. Modify connection string in App Service
       az webapp config connection-string delete --resource-group myResourceGroup --name <app-name> --setting-names MyDbConnection
