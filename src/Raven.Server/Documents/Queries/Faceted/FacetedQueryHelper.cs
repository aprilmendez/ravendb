﻿using System;
using System.Collections.Generic;
using Lucene.Net.Util;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Queries.Facets;
using Raven.Client.Extensions;
using Raven.Server.Documents.Indexes;
using Constants = Raven.Client.Constants;

namespace Raven.Server.Documents.Queries.Faceted
{
    public static class FacetedQueryHelper
    {
        private static readonly Dictionary<Type, RangeType> NumericalTypes = new Dictionary<Type, RangeType>
        {
            { typeof(decimal), RangeType.Double },
            { typeof(int), RangeType.Long },
            { typeof(long), RangeType.Long },
            { typeof(short), RangeType.Long },
            { typeof(float), RangeType.Double },
            { typeof(double), RangeType.Double }
        };

        public static bool IsAggregationNumerical(FacetAggregation aggregation)
        {
            switch (aggregation)
            {
                case FacetAggregation.Average:
                case FacetAggregation.Count:
                case FacetAggregation.Max:
                case FacetAggregation.Min:
                case FacetAggregation.Sum:
                    return true;
                default:
                    return false;
            }
        }

        public static RangeType GetRangeTypeForAggregationType(string aggregationType)
        {
            if (aggregationType == null)
                return RangeType.None;
            var type = Type.GetType(aggregationType, false, true);
            if (type == null)
                return RangeType.None;

            RangeType rangeType;
            if (NumericalTypes.TryGetValue(type, out rangeType) == false)
                return RangeType.None;

            return rangeType;
        }

        public static string GetRangeName(string field, string text, Dictionary<string, IndexField> fields)
        {
            var sortOptions = GetSortOptionsForFacet(field, fields);
            switch (sortOptions)
            {
                case SortOptions.None:
                case SortOptions.String:
                case SortOptions.StringVal:
                    //case SortOptions.Custom: // TODO [arek]
                    return text;
                //case SortOptions.NumericLong:
                //    if (IsStringNumber(text))
                //        return text;
                //    return NumericUtils.PrefixCodedToLong(text).ToInvariantString();
                //case SortOptions.NumericDouble:
                //    if (IsStringNumber(text))
                //        return text;
                //    return NumericUtils.PrefixCodedToDouble(text).ToInvariantString();
                default:
                    throw new ArgumentException($"Can't get range name from '{sortOptions}' sort option for '{field}' field.");
            }
        }

        public static SortOptions GetSortOptionsForFacet(string field, Dictionary<string, IndexField> fields)
        {
            if (field.EndsWith(Constants.Documents.Indexing.Fields.RangeFieldSuffix))
                field = field.Substring(0, field.Length - Constants.Documents.Indexing.Fields.RangeFieldSuffixLong.Length);

            IndexField value;
            if (fields.TryGetValue(field, out value) == false || value.SortOption.HasValue == false)
                return SortOptions.None;

            return value.SortOption.Value;
        }

        public static string TryTrimRangeSuffix(string fieldName)
        {
            return fieldName.EndsWith(Constants.Documents.Indexing.Fields.RangeFieldSuffix) ? fieldName.Substring(0, fieldName.Length - Constants.Documents.Indexing.Fields.RangeFieldSuffixLong.Length) : fieldName;
        }

        public static bool IsStringNumber(string value)
        {
            return string.IsNullOrEmpty(value) == false && char.IsDigit(value[0]);
        }
    }
}