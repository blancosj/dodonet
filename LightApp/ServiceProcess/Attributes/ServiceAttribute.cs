using System;

/*
 Copyright (c) 2005, 2006 David Hoyt
 
This is entirely original and provides a mechanism to find out information about
our service through reflection at runtime.
*/
namespace LightApp.ServiceProcess.Attributes
{
    #region Public Enums
    [Flags]
    public enum ServiceControls
    {
        ///<summary>The service will respond to start and stop commands.</summary>
        StartAndStop = 1,
        ///<summary>The service will respond to pause and continue commands.</summary>
        PauseAndContinue = 2,
        ///<summary>The service will respond to shutdown commands.</summary>
        Shutdown = 4,
        ///<summary>The service will respond to start, stop, pause, and continue commands.</summary>
        Default = StartAndStop | PauseAndContinue
    }

    public enum ServiceType
    {
        ///<summary>Driver service.</summary>
        KernelDriver = 0x1,
        ///<summary>File system driver service. </summary>
        FileSystemDriver = 0x2,
        ///<summary>Service that runs in its own process.</summary>
        OwnProcess = 0x10,
        ///<summary>Service that shares a process with one or more other services.</summary>
        ShareProcess = 0x20,
        ///<summary>The default value.</summary>
        Default = OwnProcess, /* A combo of Win32OwnProcess and Win32ShareProcess */
        ///<summary>The service can interact with the desktop. If the service is running in the context of the LocalSystem account, then you can use this.</summary>
        InteractiveProcessOwn = 0x100 | OwnProcess,
        ///<summary>The service can interact with the desktop. If the service is running in the context of the LocalSystem account, then you can use this.</summary>
        InteractiveProcessShare = 0x100 | ShareProcess,
        ///<summary>Do nothing.</summary>
        NoChange = ServicesAPI.SERVICE_NO_CHANGE
    }

    public enum ServiceAccessType : int
    {
        ///<summary>Required to call the QueryServiceConfig and QueryServiceConfig2 functions to query the service configuration.</summary>
        QueryConfig = 0x1,
        ///<summary>Required to call the ChangeServiceConfig or ChangeServiceConfig2 function to change the service configuration. Because this grants the caller the right to change the executable file that the system runs, it should be granted only to administrators.</summary>
        ChangeConfig = 0x2,
        ///<summary>Required to call the QueryServiceStatusEx function to ask the service control manager about the status of the service.</summary>
        QueryStatus = 0x4,
        ///<summary>Required to call the EnumDependentServices function to enumerate all the services dependent on the service.</summary>
        EnumerateDependents = 0x8,
        ///<summary>Required to call the StartService function to start the service.</summary>
        Start = 0x10,
        ///<summary>Required to call the ControlService function to stop the service.</summary>
        Stop = 0x20,
        ///<summary>Required to call the ControlService function to pause or continue the service.</summary>
        PauseContinue = 0x40,
        ///<summary>Required to call the ControlService function to ask the service to report its status immediately.</summary>
        Interrogate = 0x80,
        ///<summary>Required to call the ControlService function to specify a user-defined control code.</summary>
        UserDefinedControl = 0x100,
        ///<summary>The default value. Includes STANDARD_RIGHTS_REQUIRED in addition to all access rights in this table.</summary>
        AllAccess = ServicesAPI.STANDARD_RIGHTS_REQUIRED | QueryConfig | ChangeConfig | QueryStatus | EnumerateDependents | Start | Stop | PauseContinue | Interrogate | UserDefinedControl
    }

    public enum ServiceStartType : int
    {
        ///<summary>A device driver started by the system loader. This value is valid only for driver services.</summary>
        BootStart = 0x0,
        ///<summary>A device driver started by the IoInitSystem function. This value is valid only for driver services.</summary>
        SystemStart = 0x1,
        ///<summary>The default value. A service started automatically by the service control manager during system startup.</summary>
        AutoStart = 0x2,
        ///<summary>A service started by the service control manager when a process calls the StartService function.</summary>
        DemandStart = 0x3,
        ///<summary>A service that cannot be started. Attempts to start the service result in the error code ERROR_SERVICE_DISABLED.</summary>
        Disabled = 0x4,
        ///<summary>Do nothing.</summary>
        NoChange = ServicesAPI.SERVICE_NO_CHANGE
    }

    public enum ServiceErrorControl : int
    {
        ///<summary>The startup program logs the error but continues the startup operation.</summary>
        Ignore = 0x0,
        ///<summary>The default value. The startup program logs the error and puts up a message box pop-up but continues the startup operation.</summary>
        Normal = 0x1,
        ///<summary>The startup program logs the error. If the last-known-good configuration is being started, the startup operation continues. Otherwise, the system is restarted with the last-known-good configuration.</summary>
        Severe = 0x2,
        ///<summary>The startup program logs the error, if possible. If the last-known-good configuration is being started, the startup operation fails. Otherwise, the system is restarted with the last-known good configuration.</summary>
        Critical = 0x3,
        ///<summary>(No description available)</summary>
        MSIVital = 0x8000,
        ///<summary>Do nothing.</summary>
        NoChange = ServicesAPI.SERVICE_NO_CHANGE
    }
    #endregion

    ///<summary>Describes a new Win32 service.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ServiceAttribute : System.Attribute
    {
        #region Private Variables
        private string name;
        private string displayName;
        private string description;
        private string logName;
        private bool run;
        private ServiceType servType;
        private ServiceAccessType servAccessType;
        private ServiceStartType servStartType;
        private ServiceErrorControl servErrorControl;
        private ServiceControls servControls;
        #endregion

        #region Constructors
        ///<summary>Describes a new Win32 service.</summary>
        /// <param name="Name">The name of the service used in the service database.</param>
        /// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
        /// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
        /// <param name="Run">Indicates if you want the service to run or not on program startup.</param>
        /// <param name="ServiceType">Indicates the type of service you will be running. By default this is "Default."</param>
        /// <param name="ServiceAccessType">Access to the service. Before granting the requested access, the system checks the access token of the calling process.</param>
        /// <param name="ServiceStartType">Service start options. By default this is "AutoStart."</param>
        /// <param name="ServiceErrorControl">Severity of the error, and action taken, if this service fails to start.</param>
        /// <param name="ServiceControls">The controls or actions the service responds to.</param>
        public ServiceAttribute(string Name, string DisplayName, string Description, bool Run, ServiceType ServiceType, ServiceAccessType ServiceAccessType, ServiceStartType ServiceStartType, ServiceErrorControl ServiceErrorControl, ServiceControls ServiceControls)
        {
            this.name = Name;
            this.displayName = DisplayName;
            this.description = Description;
            this.run = Run;
            this.servType = ServiceType;
            this.servAccessType = ServiceAccessType.AllAccess;
            this.servStartType = ServiceStartType;
            this.servErrorControl = ServiceErrorControl.Normal;
            this.servControls = ServiceControls;
            this.logName = "Services";
        }

        ///<summary>Describes a new Win32 service.</summary>
        /// <param name="Name">The name of the service used in the service database.</param>
        /// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
        /// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
        /// <param name="Run">Indicates if you want the service to run or not on program startup.</param>
        /// <param name="ServiceType">Indicates the type of service you will be running. By default this is "Default."</param>
        /// <param name="ServiceAccessType">Access to the service. Before granting the requested access, the system checks the access token of the calling process.</param>
        /// <param name="ServiceStartType">Service start options. By default this is "AutoStart."</param>
        /// <param name="ServiceErrorControl">Severity of the error, and action taken, if this service fails to start.</param>
        public ServiceAttribute(string Name, string DisplayName, string Description, bool Run, ServiceType ServiceType, ServiceAccessType ServiceAccessType, ServiceStartType ServiceStartType, ServiceErrorControl ServiceErrorControl)
        {
            this.name = Name;
            this.displayName = DisplayName;
            this.description = Description;
            this.run = Run;
            this.servType = ServiceType;
            this.servAccessType = ServiceAccessType.AllAccess;
            this.servStartType = ServiceStartType;
            this.servErrorControl = ServiceErrorControl.Normal;
            this.servControls = ServiceControls.Default;
            this.logName = "Services";
        }

        ///<summary>Describes a new Win32 service.</summary>
        /// <param name="Name">The name of the service used in the service database.</param>
        /// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
        /// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
        /// <param name="ServiceType">Indicates the type of service you will be running. By default this is "Default."</param>
        /// <param name="ServiceAccessType">Access to the service. Before granting the requested access, the system checks the access token of the calling process.</param>
        /// <param name="ServiceStartType">Service start options. By default this is "AutoStart."</param>
        /// <param name="ServiceErrorControl">Severity of the error, and action taken, if this service fails to start.</param>
        public ServiceAttribute(string Name, string DisplayName, string Description, ServiceType ServiceType, ServiceAccessType ServiceAccessType, ServiceStartType ServiceStartType, ServiceErrorControl ServiceErrorControl)
        {
            this.name = Name;
            this.displayName = DisplayName;
            this.description = Description;
            this.run = true;
            this.servType = ServiceType;
            this.servAccessType = ServiceAccessType.AllAccess;
            this.servStartType = ServiceStartType;
            this.servErrorControl = ServiceErrorControl.Normal;
            this.servControls = ServiceControls.Default;
            this.logName = "Services";
        }

        ///<summary>Describes a new Win32 service.</summary>
        /// <param name="Name">The name of the service used in the service database.</param>
        /// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
        /// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
        /// <param name="ServiceType">Indicates the type of service you will be running. By default this is "Default."</param>
        /// <param name="ServiceStartType">Service start options. By default this is "AutoStart."</param>
        public ServiceAttribute(string Name, string DisplayName, string Description, ServiceType ServiceType, ServiceStartType ServiceStartType)
        {
            this.name = Name;
            this.displayName = DisplayName;
            this.description = Description;
            this.run = true;
            this.servType = ServiceType;
            this.servAccessType = ServiceAccessType.AllAccess;
            this.servStartType = ServiceStartType;
            this.servErrorControl = ServiceErrorControl.Normal;
            this.servControls = ServiceControls.Default;
            this.logName = "Services";
        }

        ///<summary>Describes a new Win32 service.</summary>
        /// <param name="Name">The name of the service used in the service database.</param>
        /// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
        /// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
        /// <param name="ServiceStartType">Service start options. By default this is "AutoStart."</param>
        public ServiceAttribute(string Name, string DisplayName, string Description, ServiceStartType ServiceStartType)
        {
            this.name = Name;
            this.displayName = DisplayName;
            this.description = Description;
            this.run = true;
            this.servType = ServiceType.Default; ;
            this.servAccessType = ServiceAccessType.AllAccess;
            this.servStartType = ServiceStartType;
            this.servErrorControl = ServiceErrorControl.Normal;
            this.servControls = ServiceControls.Default;
            this.logName = "Services";
        }

        ///<summary>Describes a new Win32 service.</summary>
        /// <param name="Name">The name of the service used in the service database.</param>
        /// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
        /// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
        /// <param name="ServiceType">Indicates the type of service you will be running. By default this is "Default."</param>
        public ServiceAttribute(string Name, string DisplayName, string Description, ServiceType ServiceType)
        {
            this.name = Name;
            this.displayName = DisplayName;
            this.description = Description;
            this.run = true;
            this.servType = ServiceType;
            this.servAccessType = ServiceAccessType.AllAccess;
            this.servStartType = ServiceStartType.AutoStart;
            this.servErrorControl = ServiceErrorControl.Normal;
            this.servControls = ServiceControls.Default;
            this.logName = "Services";
        }

        ///<summary>Describes a new Win32 service.</summary>
        /// <param name="Name">The name of the service used in the service database.</param>
        /// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
        /// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
        /// <param name="Run">Indicates if you want the service to run or not on program startup.</param>
        /// <param name="ServiceType">Indicates the type of service you will be running. By default this is "Default."</param>
        public ServiceAttribute(string Name, string DisplayName, string Description, bool Run, ServiceType ServiceType)
        {
            this.name = Name;
            this.displayName = DisplayName;
            this.description = Description;
            this.run = Run;
            this.servType = ServiceType;
            this.servAccessType = ServiceAccessType.AllAccess;
            this.servStartType = ServiceStartType.AutoStart;
            this.servErrorControl = ServiceErrorControl.Normal;
            this.servControls = ServiceControls.Default;
            this.logName = "Services";
        }

        ///<summary>Describes a new Win32 service.</summary>
        /// <param name="Name">The name of the service used in the service database.</param>
        /// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
        /// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
        /// <param name="Run">Indicates if you want the service to run or not on program startup.</param>
        public ServiceAttribute(string Name, string DisplayName, string Description, bool Run)
        {
            this.name = Name;
            this.displayName = DisplayName;
            this.description = Description;
            this.run = Run;
            this.servType = ServiceType.Default;
            this.servAccessType = ServiceAccessType.AllAccess;
            this.servStartType = ServiceStartType.AutoStart;
            this.servErrorControl = ServiceErrorControl.Normal;
            this.servControls = ServiceControls.Default;
            this.logName = "Services";
        }

        ///<summary>Describes a new Win32 service.</summary>
        /// <param name="Name">The name of the service used in the service database.</param>
        /// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
        /// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
        public ServiceAttribute(string Name, string DisplayName, string Description)
        {
            this.name = Name;
            this.displayName = DisplayName;
            this.description = Description;
            this.run = true;
            this.servType = ServiceType.Default;
            this.servAccessType = ServiceAccessType.AllAccess;
            this.servStartType = ServiceStartType.AutoStart;
            this.servErrorControl = ServiceErrorControl.Normal;
            this.servControls = ServiceControls.Default;
            this.logName = "Services";
        }

        ///<summary>Describes a new Win32 service.</summary>
        /// <param name="Name">The name of the service used in the service database.</param>
        /// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
        public ServiceAttribute(string Name, string DisplayName)
        {
            this.name = Name;
            this.displayName = DisplayName;
            this.description = "";
            this.run = true;
            this.servType = ServiceType.Default;
            this.servAccessType = ServiceAccessType.AllAccess;
            this.servStartType = ServiceStartType.AutoStart;
            this.servErrorControl = ServiceErrorControl.Normal;
            this.servControls = ServiceControls.Default;
            this.logName = "Services";
        }

        ///<summary>Describes a new Win32 service.</summary>
        /// <param name="Name">The name of the service used in the service database.</param>
        public ServiceAttribute(string Name)
        {
            this.name = Name;
            this.displayName = Name; //If no display name is specified, then make it the same as the name...
            this.description = "";
            this.run = true;
            this.servType = ServiceType.Default;
            this.servAccessType = ServiceAccessType.AllAccess;
            this.servStartType = ServiceStartType.AutoStart;
            this.servErrorControl = ServiceErrorControl.Normal;
            this.servControls = ServiceControls.Default;
            this.logName = "Services";
        }
        #endregion

        #region Properties
        ///<summary>The name of the service used in the service database.</summary>
        public string Name { get { return this.name; } set { this.Name = value; } }
        ///<summary>The name of the service that will be displayed in the services snap-in.</summary>
        public string DisplayName { get { return this.displayName; } set { this.displayName = value; } }
        ///<summary>The description of the service that will be displayed in the service snap-in.</summary>
        public string Description { get { return this.description; } set { this.description = value; } }
        ///<summary>Indicates if you want the service to run or not on program startup.</summary>
        public bool Run { get { return this.run; } set { this.run = value; } }
        ///<summary>Indicates the type of service you want to run.</summary>
        public ServiceType ServiceType { get { return this.servType; } set { this.servType = value; } }
        ///<summary>Access to the service. Before granting the requested access, the system checks the access token of the calling process.</summary>
        public ServiceAccessType ServiceAccessType { get { return this.servAccessType; } set { this.servAccessType = value; } }
        ///<summary>Service start options.</summary>
        public ServiceStartType ServiceStartType { get { return this.servStartType; } set { this.servStartType = value; } }
        ///<summary>Severity of the error, and action taken, if this service fails to start.</summary>
        public ServiceErrorControl ServiceErrorControl { get { return this.servErrorControl; } set { this.servErrorControl = value; } }
        ///<summary>The controls or actions the service responds to.</summary>
        public ServiceControls ServiceControls { get { return this.servControls; } set { this.servControls = value; } }
        ///<summary>The name of the log you want to write to.</summary>
        public string LogName { get { return this.logName; } set { this.logName = value; } }
        #endregion
    }
}
