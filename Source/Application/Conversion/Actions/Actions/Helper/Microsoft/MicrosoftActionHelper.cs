using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions.WebMail;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Helper.Microsoft;

public class MicrosoftActionHelper
{
    private const int ChunkSize = 1024 * 1024 * 4;

    private readonly HttpClient _httpClient;
    private readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public MicrosoftActionHelper()
    {
        _httpClient = new HttpClient();
    }

    public void SetupHttpClient(HttpClient httpClient,string accessToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public async Task<FileUploadResult> UploadFile(string uploadSessionUrl,string filePath, string accessToken, bool needsUploadSessionMessages = false)
    {
        SetupHttpClient(_httpClient, accessToken);

        var file = File.OpenRead(filePath);
        try
        {
            HttpContent payload;
            if (needsUploadSessionMessages)
            {
                var fInfo = new FileInfo(filePath);
                var message = CreateUploadSessionMessage(fInfo, file);
                payload = CreateJsonStringContent(JsonConvert.SerializeObject(message));
            }
            else
            {
                payload = new StringContent(string.Empty);
            }

            var createUploadSessionResponse = await _httpClient.PostAsync(uploadSessionUrl, payload);
            var readAsStringAsync = await createUploadSessionResponse.Content.ReadAsStringAsync();
            var createUploadSessionResponseContent = JsonConvert.DeserializeObject<CreateUploadSessionResponse>(readAsStringAsync);
            _httpClient.DefaultRequestHeaders.Clear();

            var totalChunks = ((int)file.Length) / ChunkSize;
            for (var i = 0; i <= totalChunks; i++)
            {
                var chunkStartingPosition = i * ChunkSize;
                var chunkArraySize = (int)Math.Min(file.Length - chunkStartingPosition, ChunkSize); 
                var lastArrayIndex = chunkStartingPosition + chunkArraySize - 1;
                var buffer = new byte[chunkArraySize];
                await file.ReadAsync(buffer, 0, chunkArraySize);

                var contentPiece = new ByteArrayContent(buffer, 0, chunkArraySize);
                contentPiece.Headers.ContentRange = new ContentRangeHeaderValue(chunkStartingPosition, lastArrayIndex, file.Length);

                var uploadResponse = await _httpClient.PutAsync(createUploadSessionResponseContent.UploadUrl, contentPiece);
                if (!uploadResponse.IsSuccessStatusCode)
                    return null;

                if (i != totalChunks) continue;
                var resultString = await uploadResponse.Content.ReadAsStringAsync();
                if (uploadResponse.StatusCode == HttpStatusCode.Created && string.IsNullOrEmpty(resultString)) return new FileUploadResult();
                var result = JsonConvert.DeserializeObject<FileUploadResult>(resultString);
                return result;
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to upload the file.");
            return null;
        }
        finally
        {
            file.Close();
        }

        return null;
    }

    public CreateUploadSessionRequest CreateUploadSessionMessage(FileInfo fInfo, FileStream file)
    {
        return new CreateUploadSessionRequest
        {
            AttachmentItem = new GraphMailAttachmentItem()
            {
                AttachmentType = "file",
                Name = fInfo.Name,
                Size = file.Length
            }
        };
    }

    public StringContent CreateJsonStringContent(string message)
    {
        return new StringContent(message, Encoding.UTF8, "application/json");
    }
}