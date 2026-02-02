// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Adapters
{
    using DotNetNuke.Abstractions.Users;
    using DotNetNuke.Entities.Users;
    using System.Diagnostics.CodeAnalysis;

    /// <inheritdoc cref="IUserControllerAdapter"/>
    [ExcludeFromCodeCoverage]
    internal class UserControllerAdapter : IUserControllerAdapter
    {
        private readonly IUserController userController;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserControllerAdapter"/> class.
        /// </summary>
        /// <param name="userController">The underlying <see cref="IUserController"/> we are adapting to.</param>
        public UserControllerAdapter(IUserController userController)
        {
            this.userController = userController;
        }

        /// <inheritdoc/>
        public IUserInfo GetUserById(int portalId, int userId) =>
            this.userController.GetUserById(portalId, userId);
    }
}
