
Add an account key to your appsettings: <add key="recommendations:accountkey" value="YourKey" />

If you want to use the 'Frequently Bought Together build' change the key in the appSettings: <add key="recommendations:useftbbuild" value="true" />.

If the baseuri for the API changes, update the key in the appSettings: <add key="recommendations:baseuri" value="https://westus.api.cognitive.microsoft.com/recommendations/v4.0" />

If you want to use a different model name, update the key in the appSettings: <add key="recommendations:modelname" value="EPiServerCommerce" />

If you want to use a different display name for the catalog, update the key in the appSettings: <add key="recommendations:catalogdisplayname" value="EPiServer Commerce catalog" />

If you want to use a different display name for the usages, update the key in the appSettings: <add key="recommendations:usagedisplayname" value="EPiServer Commerce catalog usages" />
