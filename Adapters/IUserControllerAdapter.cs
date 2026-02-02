// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Adapters
{
    using DotNetNuke.Abstractions.Users;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines a mockable adapter over DNNs UserController.
    /// </summary>
    /// <remarks>
    /// Most DNN methods in the original deal with UserInfo instead of IUserInfo
    /// and UserInfo runs code for many of it's properties that don't work in unit tests.</remarks>
    public interface IUserControllerAdapter
    {
        /// <summary>
        /// Gets a single user.
        /// </summary>
        /// <param name="portalId">The portal to get the user from.</param>
        /// <param name="userId">The ID of the user to get.</param>
        /// <returns><see cref="IUserInfo"/>.</returns>
        IUserInfo GetUserById(int portalId, int userId);
    }
}
