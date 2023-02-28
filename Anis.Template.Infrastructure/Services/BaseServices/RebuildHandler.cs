using Anis.DailyRegionSales.Application.Contracts.Repositories;
using Anis.DailyRegionSales.Application.Contracts.Services.BaseService;
using Anis.DailyRegionSales.Domain.Entities;
using Anis.DailyRegionSales.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anis.DailyRegionSales.Infra.Services.BaseService
{
    public class RebuildHandler : IRebuildHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public RebuildHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> HandleAsync(List<NotificationModel> notifications)
        {
            await HandleNewNotificationAsync(notifications);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task HandleNewNotificationAsync(List<NotificationModel> notifications)
        {
            var foundCardSales = await _unitOfWork.RegionSales.FindAsync(notifications);


            foreach (var cardSales in foundCardSales)
            {
                var models = notifications.Where(i =>
                    i.Data.Purchase.Currency == cardSales.PurchaseCurrency &&
                    i.Data.Region.Id == cardSales.RegionId &&
                    i.Data.SoldAt.Date == cardSales.Date
                    );

                foreach (var model in models)
                {
                    cardSales.IncrementTotalCosts(model);

                    cardSales.Region.Modify(model.Data.Region);

                    var notification = new Notification(model);

                    await _unitOfWork.Notifications.AddAsync(notification);
                }
            }

            var notFoundCardSalesNotifications = notifications.Where(i =>
                                    !foundCardSales.Any(c =>
                                                            i.Data.Purchase.Currency == c.PurchaseCurrency &&
                                                            i.Data.Region.Id == c.RegionId &&
                                                            i.Data.SoldAt.Date == c.Date
                                                        )).ToList();

            var regions = await _unitOfWork.Regions.FindAsync(notFoundCardSalesNotifications);

            var groupOfRegions = notFoundCardSalesNotifications.GroupBy(n => new
            {
                n.Data.Region.Id,
                n.Data.Region.ArabicName,
                n.Data.Region.EnglishName,
            });


            var groupOfCardSales = notFoundCardSalesNotifications.GroupBy(n => new
            {
                n.Data.SoldAt.Date,
                n.Data.Purchase.Currency,
                RegionId = n.Data.Region.Id,
            });

            foreach (var groupOfRegion in groupOfRegions)
            {
                var region = regions.FirstOrDefault(i => i.Id == groupOfRegion.Key.Id);

                var regionModel = new Domain.Model.Region
                {
                    Id = groupOfRegion.Key.Id,
                    ArabicName = groupOfRegion.Key.ArabicName,
                    EnglishName = groupOfRegion.Key.EnglishName,
                };
                if (region != null)
                {
                    region.Modify(regionModel);
                }
                else
                {
                    var newRegion = new Domain.Entities.Region(regionModel);

                    await _unitOfWork.Regions.AddAsync(newRegion);
                }
            }

            foreach (var groupOfCardSale in groupOfCardSales)
            {
                var summationModel = new NotificationSummation();

                foreach (var model in groupOfCardSale)
                {
                    var notification = new Notification(model);

                    await _unitOfWork.Notifications.AddAsync(notification);

                    summationModel.Update(model);
                }

                var newCardSales = new RegionSale(summationModel);
                
                await _unitOfWork.RegionSales.AddAsync(newCardSales);
            }
        }

        private async Task HandleExistNotificationAsync(List<Notification> dbNotifications, List<NotificationModel> models)
        {
            var cardSalesList = await _unitOfWork.RegionSales.FindAsync(models);

            foreach (var dbNotification in dbNotifications)
            {
                var model = models.FirstOrDefault(i => i.ItemId == dbNotification.ItemId && i.SaleInvoiceId == dbNotification.SaleInvoiceId);

                var cardSales = cardSalesList.FirstOrDefault(i =>
                                                                i.PurchaseCurrency == model.Data.Purchase.Currency &&
                                                                i.RegionId == model.Data.Region.Id &&
                                                                i.Date.Date == model.Data.SoldAt.Date);

                if (dbNotification.BuildVersion == model.BuildVersion && dbNotification.StateVersion < model.StateVersion)
                {
                    await ApplyModifyAsync(dbNotification, model, cardSales);
                }
                else if (dbNotification.BuildVersion < model.BuildVersion)
                {
                    await ApplyModifyAsync(dbNotification, model, cardSales);
                }
            }
        }

        private async Task ApplyModifyAsync(Notification dbNotification, NotificationModel model, RegionSale cardSales)
        {
            cardSales.UpdateInfo(model, dbNotification);

            dbNotification.Modify(model);

            cardSales.Region.Modify(model.Data.Region);

            await Task.CompletedTask;
        }
    }
}