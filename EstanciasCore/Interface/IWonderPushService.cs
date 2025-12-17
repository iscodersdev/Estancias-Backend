using DAL.Models.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EstanciasCore.Interface
{
    public interface IWonderPushService
    {
        Task<bool> EnviarNotificacionPorCumpleanios();
        Task<bool> EnviarNotificacionGeneral(NotificacionViewModelDTO notificacion);
        Task<bool> EnviarNotificacionAIds(NotificacionViewModelDTO notificacion, List<string> deviceIds);
    }
}
