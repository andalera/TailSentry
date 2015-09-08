namespace Tests
{
    public interface INotification
    {
        string Name { get; set; }
        bool Active { get; set; }
        string Pattern { get; set; }
        PatternType PatternType { get; set; }
        string SoundToPlayPath { get; set; }
        string TextToConvertToSpeech { get; set; }
    }
}