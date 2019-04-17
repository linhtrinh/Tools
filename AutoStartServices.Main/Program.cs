using System;
using System.ServiceProcess;
using System.Linq;
using System.Threading;

namespace AutoStartServices.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new Program();
            var timer = new Timer(app.StartDevServices, null, 0, 5 * 60 * 1000);
            for(; ; )
            {
                Thread.Sleep(5 * 60 * 1000 - 500);
            }
        }

        private void StartDevServices(Object stateInfo)
        {
            Console.WriteLine("Auto start dev services");
            StartService(Consts.SQLSERVICE);
            StartService(Consts.SQLAGENTSERVICE);
            StartService(Consts.WEBSERVICE);
            Console.WriteLine("started dev services");
        }
        private void StartService(string serviceName)
        {
            try
            {
                var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName);
                //change service start type to automatic
                if (service.StartType == ServiceStartMode.Disabled)
                {
                    Console.WriteLine($"Service {serviceName} was disabled. Try to enable it.");
                    ServiceHelper.ChangeStartMode(service, ServiceStartMode.Automatic);
                    Console.WriteLine($"Service {serviceName} was enabled.");
                }
                //start service if it stopped
                if(service.Status == ServiceControllerStatus.Stopped || 
                    service.Status == ServiceControllerStatus.Paused)
                {
                    Console.WriteLine($"Service {serviceName} was stopped. Try to start it.");
                    service.Start();
                    Console.WriteLine($"Service {serviceName} was started.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
