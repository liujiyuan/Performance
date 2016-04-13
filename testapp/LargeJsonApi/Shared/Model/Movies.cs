// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace LargeJsonApi.Model.Movies
{
    public class Properties
    {
        public bool IsPromotion { get; set; }
    }

    public class ClientConditions
    {
        public List<string> DeviceTypes { get; set; }
    }

    public class Conditions
    {
        public ClientConditions ClientConditions { get; set; }
        public string EndDate { get; set; }
        public string StartDate { get; set; }
        public List<string> ResourceSetIds { get; set; }
    }

    public class ReportingData
    {
        public string LegacyOfferInstanceId { get; set; }
    }

    public class Price
    {
        public double ListPrice { get; set; }
        public string CurrencyCode { get; set; }
        public double? MSRP { get; set; }
        public double? WholesalePrice { get; set; }
        public string WholesaleCurrencyCode { get; set; }
        public bool IsPIRequired { get; set; }
        public string TaxType { get; set; }
        public object RecurrencePrice { get; set; }
    }

    public class OrderManagementData
    {
        public string AcceptablePIPolicy { get; set; }
        public List<object> Discounts { get; set; }
        public Price Price { get; set; }
        public int GracePeriodDurationInSeconds { get; set; }
    }

    public class Availability
    {
        public Properties Properties { get; set; }
        public Conditions Conditions { get; set; }
        public ReportingData ReportingData { get; set; }
        public string AvailabilityASchema { get; set; }
        public string AvailabilityBSchema { get; set; }
        public string AvailabilityId { get; set; }
        public string LastModifiedDate { get; set; }
        public List<string> Actions { get; set; }
        public string SkuId { get; set; }
        public OrderManagementData OrderManagementData { get; set; }
        public List<string> Markets { get; set; }
    }

    public class ExtendedEntitlementData
    {
        public string EntitlementKey { get; set; }
        public string OfferId { get; set; }
        public string OfferInstanceId { get; set; }
    }

    public class PIFilter
    {
        public List<string> ExclusionProperties { get; set; }
    }

    public class OrderManagementData2
    {
        public List<ExtendedEntitlementData> ExtendedEntitlementData { get; set; }
        public List<object> FinancialInstructionDefinitions { get; set; }
        public PIFilter PIFilter { get; set; }
    }

    public class MarketProperty
    {
        public OrderManagementData2 OrderManagementData { get; set; }
        public object GrantedExternalEntitlements { get; set; }
        public object SatisfyingExternalEntitlements { get; set; }
        public object BundleProducts { get; set; }
        public List<string> Markets { get; set; }
    }

    public class OverlaysProperty
    {
        public string Type { get; set; }
        public string LocaleCode { get; set; }
        public string DisplayName { get; set; }
    }

    public class AvailableVideoLanguage
    {
        public string AudioLocale { get; set; }
        public object BurntInSubtitlesLocale { get; set; }
        public List<OverlaysProperty> OverlaysProperties { get; set; }
    }

    public class VideoAttributes
    {
        public string PrimaryResolutionFormat { get; set; }
        public string PrimaryResolutionDetail { get; set; }
        public List<AvailableVideoLanguage> AvailableVideoLanguages { get; set; }
        public List<string> AvailableDeliveryFormat { get; set; }
    }

    public class DeviceConstraint
    {
        public string DeviceType { get; set; }
        public string ConstraintType { get; set; }
        public string ConstraintValue { get; set; }
    }

    public class FulfillmentData
    {
        public VideoAttributes VideoAttributes { get; set; }
        public bool IsPlaybackGeofenced { get; set; }
        public object AcquisitionPeriodInHours { get; set; }
        public object ViewingPeriodInHours { get; set; }
        public string DistributionRight { get; set; }
        public List<DeviceConstraint> DeviceConstraints { get; set; }
    }

    public class LegacyWindowsEntitlementMetadata
    {
        public string InAppOfferToken { get; set; }
        public string InAppId { get; set; }
        public string WindowsStoreProductId { get; set; }
    }

    public class LegacyWindowsPhoneEntitlementMetadata
    {
        public string InAppOfferToken { get; set; }
        public string InAppId { get; set; }
        public string WindowsPhoneProductId { get; set; }
    }

    public class EntitlementMetadata
    {
        public string ProductType { get; set; }
        public string ProductId { get; set; }
        public string SkuId { get; set; }
        public object ConsumableQuantity { get; set; }
        public object DurationInSeconds { get; set; }
        public string SkuType { get; set; }
        public string RenewToProductId { get; set; }
        public string RenewToSkuId { get; set; }
        public List<object> Tags { get; set; }
        public string PackageFamilyName { get; set; }
        public string EntitlementKey { get; set; }
        public LegacyWindowsEntitlementMetadata LegacyWindowsEntitlementMetadata { get; set; }
        public LegacyWindowsPhoneEntitlementMetadata LegacyWindowsPhoneEntitlementMetadata { get; set; }
        public object AllowedBeneficiaries { get; set; }
    }

    public class OrderManagementData3
    {
        public List<EntitlementMetadata> EntitlementMetadata { get; set; }
        public bool IsPurchaseGeofenced { get; set; }
        public bool IsProductManagedByMicrosoft { get; set; }
        public string FinancialInstructionId { get; set; }
        public object OrderManagementPolicyId { get; set; }
        public string OrderManagementBlob { get; set; }
        public string ProductTaxCode { get; set; }
    }

    public class Properties2
    {
        public FulfillmentData FulfillmentData { get; set; }
        public object LicensingData { get; set; }
        public OrderManagementData3 OrderManagementData { get; set; }
        public object VideoInstances { get; set; }
        public List<string> LegacyEntitlementKeys { get; set; }
        public object ExternalEntitlementData { get; set; }
    }

    public class Sku
    {
        public string LastModifiedDate { get; set; }
        public List<object> LocalizedProperties { get; set; }
        public List<MarketProperty> MarketProperties { get; set; }
        public string ProductId { get; set; }
        public Properties2 Properties { get; set; }
        public string SkuASchema { get; set; }
        public string SkuBSchema { get; set; }
        public string SkuId { get; set; }
        public string SkuType { get; set; }
        public object RecurrencePolicy { get; set; }
        public object SubscriptionPolicyId { get; set; }
    }

    public class DisplaySkuAvailability
    {
        public List<Availability> Availabilities { get; set; }
        public Sku Sku { get; set; }
    }

    public class AudioTrack
    {
        public string LocaleCode { get; set; }
        public string AudioEncoding { get; set; }
        public object Channel { get; set; }
    }

    public class Trailer
    {
        public string TrailerName { get; set; }
        public string Uri { get; set; }
        public int DurationInSeconds { get; set; }
        public string VideoEncoding { get; set; }
        public string ResolutionFormat { get; set; }
        public string ResolutionDetail { get; set; }
        public object AspectRatio { get; set; }
        public string DeliveryFormat { get; set; }
        public object BurntInSubtitleLocaleCode { get; set; }
        public object OverlayFiles { get; set; }
        public List<AudioTrack> AudioTracks { get; set; }
    }

    public class Image
    {
        public string Uri { get; set; }
        public string ImagePurpose { get; set; }
        public string BackgroundColor { get; set; }
        public string ForegroundColor { get; set; }
        public string ImagePositionInfo { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Caption { get; set; }
        public int FileSizeInBytes { get; set; }
        public string UnscaledImageSHA256Hash { get; set; }
        public List<int> CropSafeZone { get; set; }
    }

    public class LocalizedProperty
    {
        public object PresaleDescription { get; set; }
        public List<string> MovieGenres { get; set; }
        public string ShortDescription { get; set; }
        public object Contributors { get; set; }
        public List<Trailer> Trailers { get; set; }
        public object Awards { get; set; }
        public object ReviewSets { get; set; }
        public object RecoMetaDataItems { get; set; }
        public List<object> SearchTitles { get; set; }
        public List<Image> Images { get; set; }
        public List<object> SearchBoosts { get; set; }
        public string ProductDescription { get; set; }
        public string ProductTitle { get; set; }
        public List<object> Franchises { get; set; }
        public string Language { get; set; }
        public List<string> Markets { get; set; }
    }

    public class Image2
    {
        public string Uri { get; set; }
        public string ImagePurpose { get; set; }
        public string BackgroundColor { get; set; }
        public string ForegroundColor { get; set; }
        public string ImagePositionInfo { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Caption { get; set; }
        public int FileSizeInBytes { get; set; }
        public string UnscaledImageSHA256Hash { get; set; }
        public object CropSafeZone { get; set; }
    }

    public class Studio
    {
        public string Name { get; set; }
        public List<Image2> Images { get; set; }
    }

    public class ContentRating
    {
        public string RatingSystem { get; set; }
        public List<object> RatingDisclaimers { get; set; }
        public List<object> RatingDescriptors { get; set; }
        public string RatingId { get; set; }
    }

    public class UsageData
    {
        public string AggregateTimeSpan { get; set; }
        public double PlayCount { get; set; }
        public double PurchaseCount { get; set; }
        public double RatingCount { get; set; }
        public double AverageRating { get; set; }
        public double RentalCount { get; set; }
        public double TrialCount { get; set; }
    }

    public class BundleProduct
    {
        public string ProductId { get; set; }
        public int Position { get; set; }
    }

    public class AlternateId
    {
        public string IdType { get; set; }
        public string Value { get; set; }
    }

    public class MarketProperty2
    {
        public object PresaleFulfillmentDate { get; set; }
        public object ComingSoonPurchaseDate { get; set; }
        public object BlackoutData { get; set; }
        public string OriginalPublicationDate { get; set; }
        public string ReleaseDate { get; set; }
        public List<Studio> Studios { get; set; }
        public object Networks { get; set; }
        public List<ContentRating> ContentRatings { get; set; }
        public List<UsageData> UsageData { get; set; }
        public int MinimumUserAge { get; set; }
        public List<BundleProduct> BundleProducts { get; set; }
        public List<AlternateId> AlternateIds { get; set; }
        public List<object> RelatedProducts { get; set; }
        public List<string> Markets { get; set; }
    }

    public class Properties3
    {
        public int DurationInSeconds { get; set; }
        public bool IsVideoBundle { get; set; }
        public string VideoType { get; set; }
    }

    public class AlternateId2
    {
        public string IdType { get; set; }
        public string Value { get; set; }
    }

    public class ValidationData
    {
        public bool PassedValidation { get; set; }
        public string RevisionId { get; set; }
        public string ValidationResultUri { get; set; }
    }

    public class Product
    {
        public List<DisplaySkuAvailability> DisplaySkuAvailabilities { get; set; }
        public string LastModifiedDate { get; set; }
        public List<LocalizedProperty> LocalizedProperties { get; set; }
        public List<MarketProperty2> MarketProperties { get; set; }
        public string ProductASchema { get; set; }
        public string ProductBSchema { get; set; }
        public string ProductId { get; set; }
        public Properties3 Properties { get; set; }
        public List<AlternateId2> AlternateIds { get; set; }
        public string DomainDataVersion { get; set; }
        public string IngestionSource { get; set; }
        public bool IsMicrosoftProduct { get; set; }
        public string PreferredSkuId { get; set; }
        public string ProductType { get; set; }
        public ValidationData ValidationData { get; set; }
        public object MerchandizingTags { get; set; }
        public List<object> SandboxIdToProductId { get; set; }
    }

    public class RootObject
    {
        public List<Product> Products { get; set; }
    }

}
