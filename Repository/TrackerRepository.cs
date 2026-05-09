using devops.Helpers;

namespace devops.Repository
{
    public class TrackerRepository: ITrackerRepository
    {
        private readonly SqlHelper _sqlHelper;
        public TrackerRepository(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }




    }
}
