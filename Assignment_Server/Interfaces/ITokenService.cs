﻿using Assignment_Server.Models;

namespace Assignment_Server.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
