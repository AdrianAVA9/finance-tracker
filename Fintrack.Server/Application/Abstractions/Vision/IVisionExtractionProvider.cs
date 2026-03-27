using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Fintrack.Server.Application.DTOs.Vision;

namespace Fintrack.Server.Application.Abstractions.Vision;

public interface IVisionExtractionProvider
{
    /// <summary>
    /// Analyzes an image stream to extract receipt/invoice data, adhering to a predefined list of valid categories.
    /// </summary>
    /// <param name="imageStream">The stream of the image file.</param>
    /// <param name="mimeType">The mime type of the image.</param>
    /// <param name="validCategories">A list of allowable category names exactly as configured by the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extracted receipt data.</returns>
    Task<ExtractedReceiptData> ExtractReceiptDataAsync(
        Stream imageStream, 
        string mimeType, 
        IEnumerable<string> validCategories, 
        CancellationToken cancellationToken = default);
}
