namespace OOPaS5.Parser
{
    
        /// <summary>
        /// Представляет информацию по одному каналу телепередач.
        /// </summary>
        public partial class TvStreamInfo
        {
            /// <summary>
            /// Название канала.
            /// </summary>
            public string channelName;

            public int channelId;
            /// <summary>
            /// Записи о передачах на данном канале.
            /// </summary>
            public TvShow[] records;

            public override string ToString()
            {
                return $"{channelName} ({channelId})";
            }
        }
    
}