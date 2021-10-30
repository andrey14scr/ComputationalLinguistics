using System;

namespace ComputationalLinguistics.Core.Dto
{
    public class TextFileDto
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; }
        public string FileAnnotationPath { get; set; }
    }
}