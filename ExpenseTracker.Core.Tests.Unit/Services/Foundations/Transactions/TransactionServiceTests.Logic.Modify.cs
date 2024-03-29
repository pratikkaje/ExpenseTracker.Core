﻿// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using ExpenseTracker.Core.Models.Transactions;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseTracker.Core.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionServiceTests
    {
        [Fact]
        public async void ShouldModifyTransactionAsync()
        {
            // Given
            DateTimeOffset randomDate = GetRandomDateTimeOffset();

            Transaction someTransaction =
                CreateRandomTransaction(dates: randomDate);

            Transaction inputTransaction = someTransaction;
            inputTransaction.UpdatedDate = randomDate.AddMinutes(1);

            Transaction storageTransaction = inputTransaction;
            Transaction updatedTransaction = inputTransaction;
            Transaction expectedTransaction = updatedTransaction;
            Guid transactionId = inputTransaction.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(transactionId))
                    .ReturnsAsync(storageTransaction);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateTransactionAsync(inputTransaction))
                    .ReturnsAsync(updatedTransaction);

            // When
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.ModifyTransactionAsync(inputTransaction);

            var actualTransaction = await modifyTransactionTask.AsTask();

            // Then
            actualTransaction.Should()
                .BeEquivalentTo(expectedTransaction);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
