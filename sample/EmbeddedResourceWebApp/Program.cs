using System.IO.Compression;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var resourceAssembly = typeof(GZipCompressedEmbeddedFileProvider).Assembly;

//decompress gzip file provider
app.UseStaticFiles(new StaticFileOptions()
{
    RequestPath = "/location1",
    FileProvider = new GZipCompressedEmbeddedFileProvider(resourceAssembly, "EmbeddedResourceWebApp.Resources.html")
});

//directly write
app.MapGet("/location2/rfc7230.htm", async context =>
{
    using var contentStream = resourceAssembly.GetManifestResourceStream("EmbeddedResourceWebApp.Resources.html.rfc7230.htm")!;

    if (AcceptEncodingCheckHelper.IsGZipAccepted(context.Request.Headers.AcceptEncoding))
    {
        context.Response.Headers.ContentEncoding = "gzip";
        await contentStream.CopyToAsync(context.Response.Body, context.RequestAborted);
    }
    else
    {
        using var gzipStream = new GZipStream(contentStream, CompressionMode.Decompress);
        await gzipStream.CopyToAsync(context.Response.Body, context.RequestAborted);
    }
});

app.Run();
