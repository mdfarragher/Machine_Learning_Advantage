using Microsoft.ProjectOxford.Vision.Contract;

namespace ml_csharp_lesson2
{
    /// <summary>
    /// The result of a scan for celebrities.
    /// </summary>
    public class CelebritiesResult
    {
        /// <summary>
        /// The list of celebrities.
        /// </summary>
        public Celebrity[] Celebrities { get; set; }
    }

    /// <summary>
    /// One specific celebrity.
    /// </summary>
    public class Celebrity
    {
        /// <summary>
        /// The name of the celebrity. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The face rectangle of the celebrity. 
        /// </summary>
        public FaceRectangle FaceRectangle { get; set; }

        /// <summary>
        /// The confidence level of the celebrity match. 
        /// </summary>
        public float Confidence { get; set; }
    }
}
