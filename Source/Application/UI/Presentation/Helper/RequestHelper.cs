using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using pdfforge.LicenseValidator.Tools.Machine;
using pdfforge.PDFCreator.UI.Presentation.Helper.Interfaces;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.Helper;

public class RequestHelper(IHashUtil hashUtil, IMachineId machineIdV2Generator) : IRequestHelper
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public async Task<(bool isSuccessful, string interactionMesage)> RequestTrial(
        RequestHelperTranslation translation,
        string emailAddress,
        string product,
        string trialRequestLink = Urls.DefaultRequestTrialUrl,
        bool marketingConsent = false, // Endpoint requires marketingConsent, but later it was agreed that this property is not needed. We still need to provided it in the request though.
        bool isUpdate = false, // Can't determine this in the application, more meant for the setup.
        bool isSetup = false)
    {
        using var client = CreateHttpClient();
        client.Timeout = TimeSpan.FromSeconds(30);
        var machineId = machineIdV2Generator.GetMachineId();
        var request = new HttpRequestMessage(HttpMethod.Post, trialRequestLink)
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    email = emailAddress,
                    product_name = product,
                    marketing_consent = marketingConsent,
                    is_update = isUpdate,
                    is_setup = isSetup,
                    machine_id = machineId, 
                    check = hashUtil.GetSha256Hash($"{emailAddress}{product}{marketingConsent}{isUpdate}{isSetup}{machineId}")
                }),
                Encoding.UTF8,
                "application/json")
        };
        try
        {
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return (true, translation.TrialRequestSuccessfulMessage);
            }

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.Forbidden: // 403
                case System.Net.HttpStatusCode.Conflict: // 409
                    _logger.Warn("Trial request failed because email address is already being used for a trial.");
                    return (false, translation.TrialRequestFailedEmailAlreadyUsedMessage);
                case System.Net.HttpStatusCode.InternalServerError: // 500
                    _logger.Warn("Trial request failed with the response status 500");
                    return (false, translation.TrialRequestFailedNetworkIssueMessage);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.Error(ex, "Network error while sending email information");
            return (false, translation.TrialRequestFailedNetworkIssueMessage);
        }
        catch (TaskCanceledException ex)
        {
            _logger.Error(ex, "Email collection request timed out");
            return (false, translation.TrialRequestFailedNetworkIssueMessage);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error while sending email information");
            return (false, translation.TrialRequestFailedNetworkIssueMessage);
        }

        return (false, translation.TrialRequestFailedNetworkIssueMessage);
    }

    // This allows overriding the client in unit tests
    protected virtual HttpClient CreateHttpClient()
    {
        return new HttpClient();
    }
}
