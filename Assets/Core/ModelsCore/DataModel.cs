using System.Collections;
using Kuci.Core.DataManagement;
using Kuci.DataManagement;

namespace Kuci.Models
{
    public class DataModel : ModelBase
    {
        private const long AutoSaveTickTimeInSeconds = 30;
        public LocalDataCollection LocalDataCollection => _localDataCollection;
        public RemoteDataHandler RemoteDataHandler => _remoteDataHandler;
        
        private readonly RemoteDataHandler _remoteDataHandler = new();
        private readonly LocalDataCollection _localDataCollection;
        private long _nextSaveTime = long.MaxValue;

        public DataModel(LocalDataCollection localDataCollection)
        {
            _localDataCollection = localDataCollection;
        }
        
        protected override void OnDispose()
        {
            base.OnDispose();
            _remoteDataHandler.SaveAllData();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            if (TimeModel.CurrentUnixTimeStamp < _nextSaveTime)
            {
                return;
            }
            
            _remoteDataHandler?.SaveAllData();
            SetNextSaveTime();
        }

        protected override IEnumerator StartLoadData()
        { 
            yield return base.StartLoadData();
            _remoteDataHandler.LoadAllData();
            SetNextSaveTime();
        }

        private void SetNextSaveTime()
        {
            _nextSaveTime = (long)TimeModel.CurrentUnixTimeStamp + AutoSaveTickTimeInSeconds;
        }
    }
}
