namespace Tests
{
    public class Notification : INotification
    {
        #region INotification Members

        public string Name { get; set; }
        public bool Active { get; set; }
        public string Pattern { get; set; }
        public PatternType PatternType { get; set; }
        public string SoundToPlayPath { get; set; }
        public string TextToConvertToSpeech { get; set; }

        #endregion
    }
}