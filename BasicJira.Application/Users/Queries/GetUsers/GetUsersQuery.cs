using System;
using System.Collections.Generic;
using System.Text;

using BasicJira.Application.DTOs;
using MediatR;

namespace BasicJira.Application.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<List<UserDto>>;
