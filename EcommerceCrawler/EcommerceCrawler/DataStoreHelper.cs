using EcommerceCrawler.Dtos;
using EcommerceCrawler.Log;
using EcommerceCrawler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using Dapper;

namespace EcommerceCrawler
{
    public class DataStoreHelper
    {
        private NpgsqlDatabase _db;
        public DataStoreHelper(NpgsqlDatabase db)
        {
            _db = db;
        }
        public void CreateTable()
        {
            _db.Connect();

            _db.Execute($@" CREATE SEQUENCE IF NOT EXISTS public.product_id_seq INCREMENT 1 MINVALUE  1 MAXVALUE 9223372036854775807 START 1 CACHE 1 ");
            _db.Execute($@" CREATE SEQUENCE IF NOT EXISTS public.product_reviews_id_seq INCREMENT 1 MINVALUE  1 MAXVALUE 9223372036854775807 START 1 CACHE 1 ");
            _db.Execute($@" CREATE SEQUENCE IF NOT EXISTS public.hourly_product_details_id_seq INCREMENT 1 MINVALUE  1 MAXVALUE 9223372036854775807 START 1 CACHE 1 ");
            _db.Execute($@" CREATE SEQUENCE IF NOT EXISTS public.batches_id_seq INCREMENT 1 MINVALUE  1 MAXVALUE 9223372036854775807 START 1 CACHE 1 ");
            #region create product table
            try
            {
                _db.Execute($@"
                    CREATE TABLE IF NOT EXISTS public.product
                    (
                        id integer not null default(nextval('public.product_id_seq'::regclass)),
                        name character varying(255) ,
                        product_detail_link character varying(1000) ,
                        review_link character varying(1000) ,
                        CONSTRAINT product_pkey PRIMARY KEY (id)
                    )
                ");
                LogTrace.WriteDebugLog($"Table succesfully created. Table Name : product");
            }
            catch (Exception e)
            {
                LogTrace.WriteDebugLog(e.ToString());
            }

            #endregion

            #region create product_reviews table
            try
            {
                _db.Execute($@"
                    CREATE TABLE IF NOT EXISTS public.product_reviews
                    (
                        id integer not null default(nextval('public.product_reviews_id_seq'::regclass)),
                        product_id integer,
                        customer character varying(255) ,
                        body character varying(5000) ,
                        stars integer,
                        CONSTRAINT product_reviews_pkey PRIMARY KEY (id)
                    )
                ");
                LogTrace.WriteDebugLog($"Table succesfully created. Table Name : product_reviews");
            }
            catch (Exception e)
            {
                LogTrace.WriteDebugLog(e.ToString());
            }
            #endregion


            #region create product_details table
            try
            {
                _db.Execute($@"
                    CREATE TABLE IF NOT EXISTS public.hourly_product_details
                    (
                        id integer not null default(nextval('public.hourly_product_details_id_seq'::regclass)),
                        name character varying(255),
                        batch_id integer,
                        fetch_time timestamp(6) without time zone,
                        rating character varying(255) ,
                        product_detail_link character varying(500) ,
                        discount double precision,
                        total_price double precision,
                        CONSTRAINT product_details_pkey PRIMARY KEY (id)
                    )
                ");
                LogTrace.WriteDebugLog($"Table succesfully created. Table Name : product_reviews");
            }
            catch (Exception e)
            {
                LogTrace.WriteDebugLog(e.ToString());
            }
            #endregion

            #region create batches table
            try
            {
                _db.Execute($@"
                    CREATE TABLE IF NOT EXISTS public.batches
                    (
                        id integer not null default(nextval('public.batches_id_seq'::regclass)),
                        fetch_time timestamp(6) without time zone,
                        CONSTRAINT batches_pkey PRIMARY KEY (id)
                    )
                ");
                LogTrace.WriteDebugLog($"Table succesfully created. Table Name : product_reviews");
            }
            catch (Exception e)
            {
                LogTrace.WriteDebugLog(e.ToString());
            }
            #endregion
        }

        public void PopulateHourlyTable(ProductDto item, string connectionstring, int batchid)
        {
            Batch b = null;
            if (batchid == 0)
            {
                b = GetMostRecentBatch(connectionstring);
                batchid = b.id;
                LogTrace.WriteDebugLog($"batch id:{batchid}");
            }

            HourlyProductDetails p = new HourlyProductDetails()
            {
                name = item.name,
                product_detail_link = item.href.link,
                batch_id = batchid,
                discount = item.discount,
                fetch_time = DateTime.Now,
                rating = item.rating,
                total_price = item.totalPrice
            };
            using (NpgsqlConnection con = new NpgsqlConnection(connectionstring))
            {
                try
                {
                    con.Execute($@"Insert into public.hourly_product_details
(name,product_detail_link,batch_id,discount,rating,total_price,fetch_time)
values (@name,@product_detail_link,@batch_id,@discount,@rating,@total_price,@fetch_time) ", p);
                }
                catch (Exception e)
                {
                    LogTrace.WriteDebugLog(e.ToString());
                }
            }
        }

        public Batch GetMostRecentBatch(string connectionstring)
        {
            Batch b = null;
            using (NpgsqlConnection con = new NpgsqlConnection(connectionstring))
            {
                b = con.Query<Batch>($@"Select * from public.batches order by fetch_time desc limit 1").FirstOrDefault();
            }
            return b;
        }

        public void CreateBatch(string connectionstring)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(connectionstring))
            {
                con.Execute($@"Insert into public.batches (fetch_time) values (@fetch_time) ", new Batch()
                {
                    fetch_time = DateTime.Now
                });
            }
        }
        public int CountData(string table, string connectionstring, string whereCond)
        {
            int count = 0;
            using (NpgsqlConnection con = new NpgsqlConnection(connectionstring))
            {
                try
                {
                    count = Convert.ToInt32(con.ExecuteScalar($@"select count(*) from public.{table} where product_detail_link ='{whereCond}'"));
                }
                catch (Exception e)
                {
                    LogTrace.WriteDebugLog(e.ToString());
                }
            }
            return count;
        }
        public void PopulateTable(ProductDto item, string connectionstring)
        {
            Product p = new Product()
            {
                name = item.name,
                product_detail_link = item.href.link
            };
            using (NpgsqlConnection con = new NpgsqlConnection(connectionstring))
            {
                try
                {
                    con.Execute($@"Insert into public.product (name,product_detail_link) values (@name,@product_detail_link) ", p);
                }
                catch (Exception e)
                {
                    LogTrace.WriteDebugLog(e.ToString());
                }
            }
        }
    }
}
