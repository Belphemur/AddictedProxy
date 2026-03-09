using AddictedProxy.OneTimeMigration.Model;
using InversionOfControl.Model;

namespace AddictedProxy.Migrations.Bootstrap;

public partial class BootstrapMigration : IBootstrap, IBootstrapAutoRegister<IMigration>;
