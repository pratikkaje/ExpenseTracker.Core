﻿using ExpenseTracker.Core.Models.Users;
using ExpenseTracker.Core.Models.Users.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async void ShouldThrowValidationExceptionOnRemoveByIdIfUserIdIsInvalidAndLogItAsync()
        {
            // Given
            Guid randomId = Guid.Empty;
            Guid invalidUserId = randomId;

            var invalidUserException = 
                new InvalidUserException(parameterName: nameof(User.Id),parameterValue: invalidUserId);

            var expectedUserValidationException = 
                new UserValidationException(invalidUserException);

            // When
            ValueTask<User> removeUserByIdTask = 
                this.userService.RemoveUserByIdAsync(invalidUserId);

            var actualUserValidationException = 
                await Assert.ThrowsAsync<UserValidationException>(() => 
                    removeUserByIdTask.AsTask());

            // Then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.userManagerBrokerMock.Verify(broker => 
                broker.SelectUserByIdAsync(It.IsAny<Guid>()), 
                    Times.Never);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))), 
                        Times.Once);

            this.userManagerBrokerMock.Verify(broker => 
                broker.DeleteUserAsync(It.IsAny<User>()), 
                    Times.Never);

            this.userManagerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
