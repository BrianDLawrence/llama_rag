using Microsoft.AspNetCore.Components.Forms;
using Xunit;

namespace UrlProcessorApi.Tests.Services;

public class ContentExtractorTests
{
    [Fact]
    public void TestHTMLExtractionSimple()
    {
        ContentExtractor extractor = new();
        var htmlInput = "<html><body><p>Test Content</p></body></html>";
        var result = extractor.ExtractContentFromHtml(htmlInput);
        Assert.Equal("Test Content",result);
    }

    [Fact]
    public void TestScriptExtractionSimple()
    {
        ContentExtractor extractor = new();
        var htmlInput = "<html><script>console.log('test');</script></html>";
        var result = extractor.ExtractContentFromHtml(htmlInput);
        Assert.Equal("",result);
    }

    [Fact]
    public void TestHTMLExtractionComplex()
    {
        ContentExtractor extractor = new();
        var htmlInput = @"
        <!DOCTYPE html>
        <html>
        <head>
            <title>Test Page</title>
            <style>
                body { font-family: Arial, sans-serif; }
                .content { color: blue; }
                .hidden { display: none; }
            </style>
            <script>
                console.log('This is a test script.');
            </script>
        </head>
        <body>
            <h1>Welcome to the Test Page</h1>
            <p class='content'>This is a paragraph of text.</p>
            <div class='content'>This is a div with some content.</div>
            <div class='hidden'>This content should be hidden.</div>
            <script>
                alert('Another test script.');
            </script>
        </body>
        </html>
        ";

        var result = extractor.ExtractContentFromHtml(htmlInput);
        Assert.Equal("Welcome to the Test Page This is a paragraph of text. This is a div with some content. This content should be hidden.",result);
    }

}