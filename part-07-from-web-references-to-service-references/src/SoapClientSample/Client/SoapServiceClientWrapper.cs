using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Part07.SoapClientSample.ServiceReference;

namespace Part07.SoapClientSample.Client;

public sealed class SoapServiceClientWrapper(string url, IReadOnlyDictionary<string, string> additionalHeaders)
{
    public CookieContainer CookieContainer { get; } = new();

    public string Login(string username, string password)
    {
        using var typedClient = CreateTypedClient();
        using var context = new OperationContextScope(typedClient.InnerChannel);

        var requestMessage = new HttpRequestMessageProperty();
        foreach (var header in additionalHeaders)
        {
            requestMessage.Headers.Add(header.Key, header.Value);
        }

        OperationContext.Current!.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;

        var cookieManager = typedClient.InnerChannel.GetProperty<IHttpCookieContainerManager>();
        cookieManager.CookieContainer = CookieContainer;

        var response = typedClient.Login(new Login
        {
            username = username,
            password = password
        });

        return response.LoginResult;
    }

    private SoapServiceClient CreateTypedClient()
    {
        var endpointAddress = new EndpointAddress(url);
        Binding binding = endpointAddress.Uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
            ? CreateBasicHttpsBinding()
            : CreateBasicHttpBinding();

        return new SoapServiceClient(binding, endpointAddress);
    }

    private static void ConfigureCommonSettings(Binding binding)
    {
        if (binding is HttpBindingBase httpBinding)
        {
            httpBinding.MaxReceivedMessageSize = int.MaxValue;
            httpBinding.AllowCookies = true;
        }

        binding.OpenTimeout = TimeSpan.FromMinutes(1);
        binding.CloseTimeout = TimeSpan.FromMinutes(1);
        binding.SendTimeout = TimeSpan.FromMinutes(10);
        binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
    }

    private static BasicHttpBinding CreateBasicHttpBinding()
    {
        var binding = new BasicHttpBinding
        {
            Security =
            {
                Mode = BasicHttpSecurityMode.None,
                Transport =
                {
                    ClientCredentialType = HttpClientCredentialType.Basic,
                    ProxyCredentialType = HttpProxyCredentialType.Basic
                }
            }
        };

        ConfigureCommonSettings(binding);
        return binding;
    }

    private static BasicHttpsBinding CreateBasicHttpsBinding()
    {
        var binding = new BasicHttpsBinding
        {
            Security =
            {
                Mode = BasicHttpsSecurityMode.Transport,
                Transport =
                {
                    ClientCredentialType = HttpClientCredentialType.None,
                    ProxyCredentialType = HttpProxyCredentialType.None
                }
            }
        };

        ConfigureCommonSettings(binding);
        return binding;
    }
}
