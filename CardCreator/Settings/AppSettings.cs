namespace CardCreator.Settings
{
    public class AppSettings
    {
        public ButtonSettings[] Buttons { get; set; }
        public string CardsDirectory { get; set; } = "cards";
        public string PdfDirectory { get; set; } = "prints";
    }
}
