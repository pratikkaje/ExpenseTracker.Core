﻿// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Models.Users.Exceptions;
using System;

namespace ExpenseTracker.Core.Services.Foundations.Users
{
    public partial class UserService
    {
        private void ValidateUserOnAdd(User user)
        {
            ValidateUserIsNotNull(user);
            ValidateUserIdIsNull(user.Id);
            ValidateUserFields(user);
            ValidateInvalidAuditFields(user);
            ValidateAuditFieldsDataOnCreate(user);
            ValidateCreatedDateIsRecent(user);
        }

        private void ValidateUserOnModify(User user)
        {
            ValidateUserIsNotNull(user);
            ValidateUserIdIsNull(user.Id);
            ValidateUserFields(user);
            ValidateAuditFieldsOnModify(user);
        }

        private void ValidateUserIsNotNull(User user)
        {
            if (user == null)
            {
                throw new NullUserException();
            }
        }

        private static void ValidateUserIdIsNull(Guid userId)
        {
            if (userId == default)
            {
                throw new InvalidUserException(
                    parameterName: nameof(User.Id),
                    parameterValue: userId);
            }
        }

        private void ValidateUserFields(User user)
        {
            if (IsInvalid(user.UserName))
            {
                throw new InvalidUserException(
                    parameterName: nameof(User.UserName),
                    parameterValue: user.UserName);
            }

            if (IsInvalid(user.FirstName))
            {
                throw new InvalidUserException(
                    parameterName: nameof(User.FirstName),
                    parameterValue: user.FirstName);
            }

            if (IsInvalid(user.LastName))
            {
                throw new InvalidUserException(
                    parameterName: nameof(User.LastName),
                    parameterValue: user.LastName);
            }

            if (IsInvalid(user.CreatedDate))
            {
                throw new InvalidUserException(
                    parameterName: nameof(User.CreatedDate),
                    parameterValue: user.CreatedDate);
            }

        }

        private static void ValidateInvalidAuditFields(User user)
        {
            switch (user)
            {
                case { } when IsInvalid(user.CreatedDate):
                    throw new InvalidUserException(
                    parameterName: nameof(User.CreatedDate),
                    parameterValue: user.CreatedDate);
                case { } when IsInvalid(user.UpdatedDate):
                    throw new InvalidUserException(
                    parameterName: nameof(User.UpdatedDate),
                    parameterValue: user.UpdatedDate);
            }
        }

        private static void ValidateAuditFieldsDataOnCreate(User user)
        {
            if (user.UpdatedDate != user.CreatedDate)
            {
                throw new InvalidUserException(
                    parameterName: nameof(User.UpdatedDate),
                    parameterValue: user.UpdatedDate);
            }
        }

        private void ValidateCreatedDateIsRecent(User user)
        {
            if (IsDateNotRecent(user.CreatedDate))
            {
                throw new InvalidUserException(
                    parameterName: nameof(User.CreatedDate),
                    parameterValue: user.CreatedDate);
            }
        }

        private static bool IsInvalid(string input) => string.IsNullOrWhiteSpace(input);

        private static bool IsInvalid(DateTimeOffset input) => input == default;

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime =
                this.dateTimeBroker.GetCurrentDateTimeOffset();

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            TimeSpan oneMinute = TimeSpan.FromMinutes(1);

            return timeDifference.Duration() > oneMinute;
        }

        private static void ValidateStorageUser(User storageUser, Guid userId)
        {
            if (storageUser is null)
            {
                throw new NotFoundUserException(userId);
            }
        }

        private void ValidateAuditFieldsOnModify(User user)
        {
            switch (user)
            {
                case { } when user.UpdatedDate == user.CreatedDate:
                    throw new InvalidUserException(
                        parameterName: nameof(User.UpdatedDate),
                        parameterValue: user.UpdatedDate);

                case { } when IsDateNotRecent(user.UpdatedDate):
                    throw new InvalidUserException(
                        parameterName: nameof(User.UpdatedDate),
                        parameterValue: user.UpdatedDate);
            }
        }
    }
}
