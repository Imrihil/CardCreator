using MyWarCreator.Features.Images;
using System.Drawing;

namespace MyWarCreator.Models
{
    public class Element
    {
        public string Content { get; }
        public Image Image { get; }
        public ElementSchema ElementSchema { get; }

        public Element(IImageProvider imageProvider, string content, ElementSchema elementSchema)
        {
            Content = content;
            Image = imageProvider.Get(content);
            ElementSchema = elementSchema;
        }
    }
}
