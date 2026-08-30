using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using HotelApplication.Dtos.PaymentDtos;
using HotelApplication.Dtos.ReservationGroupDtos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public interface IReservationGroupService
    {
        Task<Response<ReservationGroupResponseDto>> CreateReservationGroupAsync(Guid userId, CreateReservationGroupRequestDto requestDto, CancellationToken cancellationToken = default);
        Task<Response<PaymentInitializeResponseDto>> InitializePaymentAsync(Guid reservationGroupId, string email, Guid userId, CancellationToken cancellationToken = default);
        Task<Response<bool>> ConfirmPaymentAsync(string reservationGroupCode, CancellationToken cancellationToken = default);
        Task<Response<ReservationGroupResponseDto>> VerifyAndCnfirmPaymentAsync(string reservationGroupCode, Guid? userId, bool isStaff = false, CancellationToken cancellationToken = default);

        Task<Response<ReservationGroupResponseDto>> GetReservationGroupByIdAsync(Guid reservationgroupid, Guid? userId, bool isStaff = false, CancellationToken cancellationToken = default);
        Task<Response<ReservationGroupResponseDto>> GetReservationGroupByCodeAsync(string code, Guid? userId, bool isStaff = false, CancellationToken cancellationToken = default);
        Task<Response<ReservationGroupResponseDto>> CancelReservationGroupAsync(Guid reservationGroupId, Guid? userId, bool isStaff, CancellationToken cancellationToken = default);
        Task<Response<IReadOnlyCollection<ReservationGroupResponseDto>>> GetAllReservationGroupsAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}