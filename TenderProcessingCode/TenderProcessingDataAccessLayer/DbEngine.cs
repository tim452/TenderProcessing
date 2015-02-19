﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TenderProcessingDataAccessLayer.Enums;
using TenderProcessingDataAccessLayer.Models;

namespace TenderProcessingDataAccessLayer
{

    public class DbEngine
    {
        private readonly string _connectionString;

        public DbEngine()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["TenderProccessing"].ConnectionString;
        }

        #region TenderClaim

        public bool SaveTenderClaim(TenderClaim model)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SaveTenderClaim";
                if (!string.IsNullOrEmpty(model.TenderNumber))
                    cmd.Parameters.AddWithValue("@tenderNumber", model.TenderNumber);
                cmd.Parameters.AddWithValue("@tenderStart", model.TenderStart);
                cmd.Parameters.AddWithValue("@claimDeadline", model.ClaimDeadline);
                cmd.Parameters.AddWithValue("@kPDeadline", model.KPDeadline);
                if (!string.IsNullOrEmpty(model.Comment))
                    cmd.Parameters.AddWithValue("@comment", model.Comment);
                cmd.Parameters.AddWithValue("@customer", model.Customer);
                cmd.Parameters.AddWithValue("@customerInn", model.CustomerInn);
                if (!model.Sum.Equals(0)) cmd.Parameters.AddWithValue("@totalSum", model.Sum);
                cmd.Parameters.AddWithValue("@dealType", model.DealType);
                if (!string.IsNullOrEmpty(model.TenderUrl))
                    cmd.Parameters.AddWithValue("@tenderUrl", model.TenderUrl);
                cmd.Parameters.AddWithValue("@tenderStatus", model.TenderStatus);
                cmd.Parameters.AddWithValue("@manager", model.Manager.Id);
                cmd.Parameters.AddWithValue("@managerSubDivision", model.Manager.SubDivision);
                cmd.Parameters.AddWithValue("@claimStatus", model.ClaimStatus);
                cmd.Parameters.AddWithValue("@recordDate", model.RecordDate);
                cmd.Parameters.AddWithValue("@deleted", model.Deleted);
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    rd.Read();
                    var id = rd.GetInt32(0);
                    if (id != -1)
                    {
                        result = true;
                        model.Id = id;
                    }
                }
                rd.Dispose();
            }
            return result;
        }

        public bool ChangeTenderClaimClaimStatus(TenderClaim model)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ChangeTenderClaimClaimStatus";
                cmd.Parameters.AddWithValue("@id", model.Id);
                cmd.Parameters.AddWithValue("@claimStatus", model.ClaimStatus);
                conn.Open();
                result = cmd.ExecuteNonQuery() > 0;
            }
            return result;
        }

        public List<TenderClaim> LoadTenderClaims(int pageSize)
        {
            var list = new List<TenderClaim>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "LoadTenderClaims";
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        var model = new TenderClaim()
                        {
                            Id = rd.GetInt32(0),
                            TenderNumber = rd.GetString(1),
                            TenderStart = rd.GetDateTime(2),
                            ClaimDeadline = rd.GetDateTime(3),
                            KPDeadline = rd.GetDateTime(4),
                            Comment = rd.GetString(5),
                            Customer = rd.GetString(6),
                            CustomerInn = rd.GetString(7),
                            Sum = (double)rd.GetDecimal(8),
                            DealType = rd.GetInt32(9),
                            TenderUrl = rd.GetString(10),
                            TenderStatus = rd.GetInt32(11),
                            Manager = new Manager()
                            {
                                Id = rd.GetString(12),
                                SubDivision = rd.GetString(13)
                            },
                            ClaimStatus = rd.GetInt32(14),
                            RecordDate = rd.GetDateTime(15),
                            ProductManagers = new List<ProductManager>()
                        };
                        if (model.Sum.Equals(-1)) model.Sum = 0;
                        model.KPDeadlineString = model.KPDeadline.ToString("dd.MM.yyyy");
                        model.TenderStartString = model.TenderStart.ToString("dd.MM.yyyy");
                        model.ClaimDeadlineString = model.ClaimDeadline.ToString("dd.MM.yyyy");
                        list.Add(model);
                    }
                }
                rd.Dispose();
            }
            return list;
        }

        public TenderClaim LoadTenderClaimById(int id)
        {
            TenderClaim model = null;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "LoadTenderClaimById";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    rd.Read();
                    model = new TenderClaim()
                    {
                        Id = rd.GetInt32(0),
                        TenderNumber = rd.GetString(1),
                        TenderStart = rd.GetDateTime(2),
                        ClaimDeadline = rd.GetDateTime(3),
                        KPDeadline = rd.GetDateTime(4),
                        Comment = rd.GetString(5),
                        Customer = rd.GetString(6),
                        CustomerInn = rd.GetString(7),
                        Sum = (double) rd.GetDecimal(8),
                        DealType = rd.GetInt32(9),
                        TenderUrl = rd.GetString(10),
                        TenderStatus = rd.GetInt32(11),
                        Manager = new Manager()
                        {
                            Id = rd.GetString(12),
                            SubDivision = rd.GetString(13)
                        },
                        ClaimStatus = rd.GetInt32(14),
                        RecordDate = rd.GetDateTime(15),
                        ProductManagers = new List<ProductManager>()
                    };
                    if (model.Sum.Equals(-1)) model.Sum = 0;
                    model.KPDeadlineString = model.KPDeadline.ToString("dd.MM.yyyy");
                    model.TenderStartString = model.TenderStart.ToString("dd.MM.yyyy");
                    model.ClaimDeadlineString = model.ClaimDeadline.ToString("dd.MM.yyyy");
                }
                rd.Dispose();
            }
            return model;
        }

        public int GetTenderClaimCount()
        {
            var count = -1;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetTenderClaimCount";
                conn.Open();
                count = (int) cmd.ExecuteScalar();
            }
            return count;
        }

        public bool DeleteTenderClaim(int id)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "DeleteTenderClaims";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                result = cmd.ExecuteNonQuery() > 0;
            }
            return result;
        }

        public List<TenderClaim> FilterTenderClaims(FilterTenderClaim filter)
        {
            var list = new List<TenderClaim>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = GetFilterTenderClaimQuery(filter, false);
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        var model = new TenderClaim()
                        {
                            Id = rd.GetInt32(0),
                            TenderNumber = rd.GetString(1),
                            TenderStart = rd.GetDateTime(2),
                            ClaimDeadline = rd.GetDateTime(3),
                            KPDeadline = rd.GetDateTime(4),
                            Comment = rd.GetString(5),
                            Customer = rd.GetString(6),
                            CustomerInn = rd.GetString(7),
                            Sum = (double)rd.GetDecimal(8),
                            DealType = rd.GetInt32(9),
                            TenderUrl = rd.GetString(10),
                            TenderStatus = rd.GetInt32(11),
                            Manager = new Manager()
                            {
                                Id = rd.GetString(12),
                                SubDivision = rd.GetString(13)
                            },
                            ClaimStatus = rd.GetInt32(14),
                            RecordDate = rd.GetDateTime(15),
                            ProductManagers = new List<ProductManager>()
                        };
                        if (model.Sum.Equals(-1)) model.Sum = 0;
                        model.KPDeadlineString = model.KPDeadline.ToString("dd.MM.yyyy");
                        model.TenderStartString = model.TenderStart.ToString("dd.MM.yyyy");
                        model.ClaimDeadlineString = model.ClaimDeadline.ToString("dd.MM.yyyy");
                        list.Add(model);
                    }
                }
                rd.Dispose();
            }
            return list;
        }

        public int GetCountFilteredTenderClaims(FilterTenderClaim filter)
        {
            var count = -1;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = GetFilterTenderClaimQuery(filter, true);
                conn.Open();
                count = (int)cmd.ExecuteScalar();
            }
            return count;
        }

        public void SetProductManagersForClaims(List<TenderClaim> claims)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                var query = "select distinct IdClaim, ProductManager from ClaimPosition where IdClaim in (" +
                            string.Join(",", claims.Select(x=>x.Id)) + ")";
                cmd.CommandText = query;
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        var idClaim = rd.GetInt32(0);
                        var model = new ProductManager() {Id = rd.GetString(1)};
                        claims.First(x => x.Id == idClaim).ProductManagers.Add(model);
                    }
                }
                rd.Dispose();
            }
        }

        private string GetFilterTenderClaimQuery(FilterTenderClaim model, bool forCount)
        {
            var sb = new StringBuilder();
            sb.Append("select ");
            if (!forCount)
            {
                if (model.RowCount > 0)
                {
                    sb.Append("top(" + model.RowCount + ") ");
                }
                sb.Append(" * from TenderClaim where deleted = 0");
            }
            else
            {
                sb.Append(" count(*) from TenderClaim where deleted = 0");
            }
            if (model.IdClaim != 0)
            {
                sb.Append(" and Id = " + model.IdClaim);
            }
            if (!string.IsNullOrEmpty(model.TenderNumber))
            {
                sb.Append(" and TenderNumber = '" + model.TenderNumber + "'");
            }
            if (model.ClaimStatus != 0)
            {
                sb.Append(" and ClaimStatus = " + model.ClaimStatus);
            }
            if (!string.IsNullOrEmpty(model.IdManager))
            {
                sb.Append(" and Manager = '" + model.IdManager + "'");
            }
            if (!string.IsNullOrEmpty(model.ManagerSubDivision))
            {
                sb.Append(" and ManagerSubDivision = '" + model.ManagerSubDivision + "'");
            }
            if (!string.IsNullOrEmpty(model.TenderStartFrom) && !string.IsNullOrEmpty(model.TenderStartTo))
            {
                var dateFrom = DateTime.ParseExact(model.TenderStartFrom, "dd.MM.yyyy", CultureInfo.CurrentCulture);
                var dateTo = DateTime.ParseExact(model.TenderStartTo, "dd.MM.yyyy", CultureInfo.CurrentCulture);
                var dateFromString = dateFrom.Year.ToString("G") +
                                     (dateFrom.Month < 10
                                         ? "0" + dateFrom.Month.ToString("G")
                                         : dateFrom.Month.ToString("G")) +
                                     (dateFrom.Day < 10 ? "0" + dateFrom.Day.ToString("G") : dateFrom.Day.ToString("G"));
                var dateToString = dateTo.Year.ToString("G") +
                                     (dateTo.Month < 10
                                         ? "0" + dateTo.Month.ToString("G")
                                         : dateTo.Month.ToString("G")) +
                                     (dateTo.Day < 10 ? "0" + dateTo.Day.ToString("G") : dateTo.Day.ToString("G"));
                sb.Append(" and ClaimDeadline BETWEEN '" + dateFromString + "' AND '" + dateToString + "'");
            }
            else
            {
                if (!string.IsNullOrEmpty(model.TenderStartFrom))
                {
                    var dateFrom = DateTime.ParseExact(model.TenderStartFrom, "dd.MM.yyyy", CultureInfo.CurrentCulture);
                    var dateFromString = dateFrom.Year.ToString("G") +
                                         (dateFrom.Month < 10
                                             ? "0" + dateFrom.Month.ToString("G")
                                             : dateFrom.Month.ToString("G")) +
                                         (dateFrom.Day < 10 ? "0" + dateFrom.Day.ToString("G") : dateFrom.Day.ToString("G"));
                    sb.Append(" and ClaimDeadline >= '" + dateFromString + "'");
                }
                if (!string.IsNullOrEmpty(model.TenderStartTo))
                {
                    var dateTo = DateTime.ParseExact(model.TenderStartTo, "dd.MM.yyyy", CultureInfo.CurrentCulture);
                    var dateToString = dateTo.Year.ToString("G") +
                                         (dateTo.Month < 10
                                             ? "0" + dateTo.Month.ToString("G")
                                             : dateTo.Month.ToString("G")) +
                                         (dateTo.Day < 10 ? "0" + dateTo.Day.ToString("G") : dateTo.Day.ToString("G"));
                    sb.Append(" and ClaimDeadline <= '" + dateToString + "'");
                }
            }
            if (model.Overdie.HasValue)
            {
                
            }
            if (!string.IsNullOrEmpty(model.IdProductManager))
            {
                sb.Append(" and '" + model.IdProductManager +
                          "' in (select ProductManager from ClaimPosition where IdClaim = [TenderClaim].Id)");
            }
            return sb.ToString();
        }

        #endregion

        #region SpecificationPosition

        public bool SaveSpecificationPosition(SpecificationPosition model)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SaveClaimPosition";
                if (!string.IsNullOrEmpty(model.Replace))
                    cmd.Parameters.AddWithValue("@replaceValue", model.Replace);
                if (!string.IsNullOrEmpty(model.CatalogNumber))
                    cmd.Parameters.AddWithValue("@catalogNumber", model.CatalogNumber);
                if (!model.Price.Equals(0)) cmd.Parameters.AddWithValue("@price", model.Price);
                if (!model.Sum.Equals(0)) cmd.Parameters.AddWithValue("@sumMax", model.Sum);
                if (model.RowNumber != 0) cmd.Parameters.AddWithValue("@rowNumber", model.RowNumber);
                cmd.Parameters.AddWithValue("@idClaim", model.IdClaim);
                cmd.Parameters.AddWithValue("@name", model.Name);
                cmd.Parameters.AddWithValue("@unit", model.Unit);
                cmd.Parameters.AddWithValue("@value", model.Value);
                cmd.Parameters.AddWithValue("@productManager", model.ProductManager.Id);
                cmd.Parameters.AddWithValue("@comment", model.Comment);
                cmd.Parameters.AddWithValue("@positionState", model.State);
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    rd.Read();
                    var id = rd.GetInt32(0);
                    if (id != -1)
                    {
                        result = true;
                        model.Id = id;
                    }
                }
                rd.Dispose();
            }
            return result;
        }

        public bool UpdateSpecificationPosition(SpecificationPosition model)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UpdateClaimPosition";
                if (!string.IsNullOrEmpty(model.Replace))
                    cmd.Parameters.AddWithValue("@replaceValue", model.Replace);
                if (!string.IsNullOrEmpty(model.CatalogNumber))
                    cmd.Parameters.AddWithValue("@catalogNumber", model.CatalogNumber);
                if (!model.Price.Equals(0)) cmd.Parameters.AddWithValue("@price", model.Price);
                if (!model.Sum.Equals(0)) cmd.Parameters.AddWithValue("@sumMax", model.Sum);
                if (model.RowNumber != 0) cmd.Parameters.AddWithValue("@rowNumber", model.RowNumber);
                cmd.Parameters.AddWithValue("@id", model.Id);
                cmd.Parameters.AddWithValue("@name", model.Name);
                cmd.Parameters.AddWithValue("@unit", model.Unit);
                cmd.Parameters.AddWithValue("@value", model.Value);
                cmd.Parameters.AddWithValue("@productManager", model.ProductManager.Id);
                cmd.Parameters.AddWithValue("@comment", model.Comment);
                cmd.Parameters.AddWithValue("@positionState", model.State);
                conn.Open();
                result = cmd.ExecuteNonQuery() > 0;
            }
            return result;
        }

        public bool ExistsSpecificationPosition(SpecificationPosition model)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ExistsClaimPosition";
                if (!string.IsNullOrEmpty(model.Replace))
                    cmd.Parameters.AddWithValue("@replaceValue", model.Replace);
                if (!string.IsNullOrEmpty(model.CatalogNumber))
                    cmd.Parameters.AddWithValue("@catalogNumber", model.CatalogNumber);
                if (!model.Price.Equals(0)) cmd.Parameters.AddWithValue("@price", model.Price);
                if (!model.Sum.Equals(0)) cmd.Parameters.AddWithValue("@sumMax", model.Sum);
                if (model.RowNumber != 0) cmd.Parameters.AddWithValue("@rowNumber", model.RowNumber);
                cmd.Parameters.AddWithValue("@idClaim", model.IdClaim);
                cmd.Parameters.AddWithValue("@name", model.Name);
                cmd.Parameters.AddWithValue("@unit", model.Unit);
                cmd.Parameters.AddWithValue("@value", model.Value);
                cmd.Parameters.AddWithValue("@productManager", model.ProductManager.Id);
                cmd.Parameters.AddWithValue("@comment", model.Comment);
                cmd.Parameters.AddWithValue("@positionState", model.State);
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    rd.Read();
                    result = rd.GetInt32(0) == 0;
                }
                rd.Dispose();
            }
            return result;
        }

        public bool DeleteSpecificationPosition(int id)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "DeleteClaimPosition";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                result = cmd.ExecuteNonQuery() > 0;
            }
            return result;
        }

        public List<SpecificationPosition> LoadSpecificationPositionsForTenderClaim(int claimId)
        {
            var list = new List<SpecificationPosition>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "LoadClaimPositionForTenderClaim";
                cmd.Parameters.AddWithValue("@id", claimId);
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        var model = new SpecificationPosition()
                        {
                            Id = rd.GetInt32(0),
                            RowNumber = rd.GetInt32(2),
                            CatalogNumber = rd.GetString(3),
                            Name = rd.GetString(4),
                            Replace = rd.GetString(5),
                            Unit = (PositionUnit)Enum.Parse(typeof(PositionUnit), rd.GetString(6)),
                            Value = rd.GetInt32(7),
                            ProductManager = new ProductManager() {Id = rd.GetString(8)},
                            Comment = rd.GetString(9),
                            Price = (double)rd.GetDecimal(10),
                            Sum = (double)rd.GetDecimal(11),
                            State = rd.GetInt32(12)
                        };
                        if (model.Sum.Equals(-1)) model.Sum = 0;
                        if (model.Price.Equals(-1)) model.Price = 0;
                        if (model.RowNumber == -1) model.RowNumber = 0;
                        list.Add(model);
                    }
                }
                rd.Dispose();
            }
            return list;
        }

        public bool ChangePositionState(int id, int state)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ChangeClaimPositionState";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@positionState", state);
                conn.Open();
                result = (int)cmd.ExecuteScalar() > 0;
            }
            return result;
        }

        public bool HasClaimPosition(int id)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "HasClaimPosition";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                result = (int) cmd.ExecuteScalar() > 0;
            }
            return result;
        }

        public bool IsPositionsReadyToConfirm(List<SpecificationPosition> positions)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                var query = "select IdPosition, count(*) from CalculateClaimPosition where IdPosition in(" +
                            string.Join(",", positions.Select(x => x.Id)) + ") group by IdPosition";
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                conn.Open();
                var list = new List<PositionCalculateCount>();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        var model = new PositionCalculateCount()
                        {
                            Id = rd.GetInt32(0),
                            Count = rd.GetInt32(1)
                        };
                        if (model.Count == 0)
                        {
                            break;
                        }
                        list.Add(model);
                    }
                    if (list.Count() == positions.Count())
                    {
                        var isNullCount = list.Any(x => x.Count == 0);
                        if (!isNullCount)
                        {
                            result = true;
                        }
                    }
                }
                rd.Dispose();
            }
            return result;
        }

        public bool SetPositionsToConfirm(List<SpecificationPosition> positions)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                var query = "update ClaimPosition set PositionState = 2 where Id in(" +
                            string.Join(",", positions.Select(x => x.Id)) + ")";
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                conn.Open();
                result = cmd.ExecuteNonQuery() > 0;
            }
            return result;
        }

        public List<ProductManager> LoadProductManagersForClaim(int claimId)
        {
            var list = new List<ProductManager>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "LoadProductManagersForClaim";
                cmd.Parameters.AddWithValue("@idClaim", claimId);
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        var model = new ProductManager()
                        {
                            Id = rd.GetString(0)
                        };
                        list.Add(model);
                    }
                }
                rd.Dispose();
            }
            return list;
        }

        #endregion

        #region CalculateSpecificationPosition

        public bool SaveCalculateSpecificationPosition(CalculateSpecificationPosition model)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SaveCalculateClaimPosition";
                if (!string.IsNullOrEmpty(model.Replace))
                    cmd.Parameters.AddWithValue("@replaceValue", model.Replace);
                if (!string.IsNullOrEmpty(model.Comment))
                    cmd.Parameters.AddWithValue("@comment", model.CatalogNumber);
                if (!string.IsNullOrEmpty(model.ProtectCondition))
                    cmd.Parameters.AddWithValue("@protectCondition", model.ProtectCondition);
                if (!string.IsNullOrEmpty(model.Provider))
                    cmd.Parameters.AddWithValue("@provider", model.Provider);
                if (!model.PriceUsd.Equals(0)) cmd.Parameters.AddWithValue("@priceUsd", model.PriceUsd);
                if (!model.PriceRub.Equals(0)) cmd.Parameters.AddWithValue("@priceRub", model.PriceRub);
                if (!model.SumUsd.Equals(0)) cmd.Parameters.AddWithValue("@sumUsd", model.SumUsd);
                cmd.Parameters.AddWithValue("@idClaim", model.IdTenderClaim);
                cmd.Parameters.AddWithValue("@idPosition", model.IdSpecificationPosition);
                cmd.Parameters.AddWithValue("@name", model.Name);
                cmd.Parameters.AddWithValue("@catalogNumber", model.CatalogNumber);
                cmd.Parameters.AddWithValue("@protectFact", model.ProtectFact);
                cmd.Parameters.AddWithValue("@sumRub", model.SumRub);
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    rd.Read();
                    var id = rd.GetInt32(0);
                    if (id > 0)
                    {
                        result = true;
                        model.Id = id;
                    }
                }
                rd.Dispose();
            }
            return result;
        }

        public bool UpdateCalculateSpecificationPosition(CalculateSpecificationPosition model)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UpdateCalculateClaimPosition";
                if (!string.IsNullOrEmpty(model.Replace))
                    cmd.Parameters.AddWithValue("@replaceValue", model.Replace);
                if (!string.IsNullOrEmpty(model.Comment))
                    cmd.Parameters.AddWithValue("@comment", model.CatalogNumber);
                if (!string.IsNullOrEmpty(model.ProtectCondition))
                    cmd.Parameters.AddWithValue("@protectCondition", model.ProtectCondition);
                if (!string.IsNullOrEmpty(model.Provider))
                    cmd.Parameters.AddWithValue("@provider", model.Provider);
                if (!model.PriceUsd.Equals(0)) cmd.Parameters.AddWithValue("@priceUsd", model.PriceUsd);
                if (!model.PriceRub.Equals(0)) cmd.Parameters.AddWithValue("@priceRub", model.PriceRub);
                if (!model.SumUsd.Equals(0)) cmd.Parameters.AddWithValue("@sumUsd", model.SumUsd);
                cmd.Parameters.AddWithValue("@id", model.Id);
                cmd.Parameters.AddWithValue("@name", model.Name);
                cmd.Parameters.AddWithValue("@catalogNumber", model.CatalogNumber);
                cmd.Parameters.AddWithValue("@protectFact", model.ProtectFact);
                cmd.Parameters.AddWithValue("@sumRub", model.SumRub);
                conn.Open();
                result = cmd.ExecuteNonQuery() > 0;
            }
            return result;
        }

        public bool DeleteCalculateSpecificationPosition(int id)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "DeleteCalculateClaimPosition";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                result = cmd.ExecuteNonQuery() > 0;
            }
            return result;
        }

        public List<CalculateSpecificationPosition> LoadCalculateSpecificationPositionsForTenderClaim(int claimId)
        {
            var list = new List<CalculateSpecificationPosition>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "LoadCalculateClaimPositionForClaim";
                cmd.Parameters.AddWithValue("@id", claimId);
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        var model = new CalculateSpecificationPosition()
                        {
                            Id = rd.GetInt32(0),
                            IdSpecificationPosition = rd.GetInt32(1),
                            IdTenderClaim = rd.GetInt32(2),
                            CatalogNumber = rd.GetString(3),
                            Name = rd.GetString(4),
                            Replace = rd.GetString(5),
                            PriceUsd = (double)rd.GetDecimal(6),
                            SumUsd = (double)rd.GetDecimal(7),
                            PriceRub = (double)rd.GetDecimal(8),
                            SumRub = (double)rd.GetDecimal(9),
                            Provider = rd.GetString(10),
                            ProtectFact = rd.GetString(11),
                            ProtectCondition = rd.GetString(12),
                            Comment = rd.GetString(13)
                        };
                        if (model.PriceUsd.Equals(-1)) model.PriceUsd = 0;
                        if (model.SumUsd.Equals(-1)) model.SumUsd = 0;
                        if (model.PriceRub.Equals(-1)) model.PriceRub = 0;
                        list.Add(model);
                    }
                }
                rd.Dispose();
            }
            return list;
        }

        public void DeleteCalculateSpecificationPositionForClaim(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "DeleteCalculatePositionForClaim";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region ClaimStatusHistory

        public bool SaveClaimStatusHistory(ClaimStatusHistory model)
        {
            var result = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SaveClaimStatusHistory";
                if (!string.IsNullOrEmpty(model.Comment))
                    cmd.Parameters.AddWithValue("@comment", model.Comment);
                cmd.Parameters.AddWithValue("@idClaim", model.IdClaim);
                cmd.Parameters.AddWithValue("@recordDate", model.Date);
                cmd.Parameters.AddWithValue("@idUser", model.IdUser);
                cmd.Parameters.AddWithValue("@idStatus", model.Status.Id);
                conn.Open();
                result = cmd.ExecuteNonQuery() > 0;
            }
            return result;
        }

        public List<ClaimStatusHistory> LoadStatusHistoryForClaim(int claimId)
        {
            var list = new List<ClaimStatusHistory>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "LoadStatusHistoryForClaim";
                cmd.Parameters.AddWithValue("@idClaim", claimId);
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        var model = new ClaimStatusHistory()
                        {
                            Id = rd.GetInt32(0),
                            Date = rd.GetDateTime(1),
                            IdClaim = rd.GetInt32(2),
                            Comment = rd.GetString(4),
                            IdUser = rd.GetString(5),
                            Status = new ClaimStatus()
                            {
                                Id = rd.GetInt32(3),
                                Value = rd.GetString(6)
                            }
                        };
                        model.DateString = model.Date.ToString("dd.MM.yyyy HH:mm");
                        list.Add(model);
                    }
                }
                rd.Dispose();
            }
            return list;
        }

        public ClaimStatusHistory LoadLastStatusHistoryForClaim(int claimId)
        {
            ClaimStatusHistory model = null;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "LoadLastStatusHistoryForClaim";
                cmd.Parameters.AddWithValue("@idClaim", claimId);
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    rd.Read();
                    model = new ClaimStatusHistory()
                    {
                        Id = rd.GetInt32(0),
                        Date = rd.GetDateTime(1),
                        IdClaim = rd.GetInt32(2),
                        Comment = rd.GetString(4),
                        IdUser = rd.GetString(5),
                        Status = new ClaimStatus()
                        {
                            Id = rd.GetInt32(3),
                            Value = rd.GetString(6)
                        }
                    };
                }
                rd.Dispose();
            }
            return model;
        }

        #endregion

        #region Справочники

        public List<DealType> LoadDealTypes()
        {
            var list = new List<DealType>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "LoadDealTypes";
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        var model = new DealType()
                        {
                            Id = rd.GetInt32(0),
                            Value = rd.GetString(1)
                        };
                        list.Add(model);
                    }
                }
                rd.Dispose();
            }
            return list;
        }

        public List<ClaimStatus> LoadClaimStatus()
        {
            var list = new List<ClaimStatus>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "LoadClaimStatus";
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        var model = new ClaimStatus()
                        {
                            Id = rd.GetInt32(0),
                            Value = rd.GetString(1)
                        };
                        list.Add(model);
                    }
                }
                rd.Dispose();
            }
            return list;
        }

        public List<TenderStatus> LoadTenderStatus()
        {
            var list = new List<TenderStatus>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "LoadTenderStatus";
                conn.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        var model = new TenderStatus()
                        {
                            Id = rd.GetInt32(0),
                            Value = rd.GetString(1)
                        };
                        list.Add(model);
                    }
                }
                rd.Dispose();
            }
            return list;
        }

        #endregion
    }
}
