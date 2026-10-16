-- ============================================================
-- DUMMY DATA untuk tes Ubah Status Penerbangan
-- Cara pakai: sqlcmd -S "(localdb)\MSSQLLocalDB" -d BromoAirlines -i DummyJadwal_UbahStatus.sql
-- Aman dijalankan ulang (cek kode dulu, skip kalau sudah ada)
-- ============================================================
USE BromoAirlines;
GO

-- Pastikan maskapai/bandara acuan ada (pakai data bawaan seed)
-- Bandara: 6=Halim(HLP), 7=Juanda(SUB), 9=Soekarno-Hatta(CGK), 10=Supadio(PNK)
-- Maskapai: 4=Batik Air, 6=Citilink, 8=Lion Air, 10=Super Air Jet

IF NOT EXISTS (SELECT 1 FROM JadwalPenerbangan WHERE KodePenerbangan = 'IU-0696')
INSERT INTO JadwalPenerbangan (KodePenerbangan, BandaraKeberangkatanID, BandaraTujuanID, MaskapaiID, TanggalWaktuKeberangkatan, DurasiPenerbangan, HargaPerTiket)
VALUES ('IU-0696', 9, 10, 10, '2026-04-21 07:25:00', 95, 850000);

IF NOT EXISTS (SELECT 1 FROM JadwalPenerbangan WHERE KodePenerbangan = 'IU-0697')
INSERT INTO JadwalPenerbangan (KodePenerbangan, BandaraKeberangkatanID, BandaraTujuanID, MaskapaiID, TanggalWaktuKeberangkatan, DurasiPenerbangan, HargaPerTiket)
VALUES ('IU-0697', 10, 9, 10, '2026-04-27 09:40:00', 95, 870000);

IF NOT EXISTS (SELECT 1 FROM JadwalPenerbangan WHERE KodePenerbangan = 'ID-7508')
INSERT INTO JadwalPenerbangan (KodePenerbangan, BandaraKeberangkatanID, BandaraTujuanID, MaskapaiID, TanggalWaktuKeberangkatan, DurasiPenerbangan, HargaPerTiket)
VALUES ('ID-7508', 7, 6, 4, '2026-05-22 05:00:00', 85, 900000);

IF NOT EXISTS (SELECT 1 FROM JadwalPenerbangan WHERE KodePenerbangan = 'ID-6589')
INSERT INTO JadwalPenerbangan (KodePenerbangan, BandaraKeberangkatanID, BandaraTujuanID, MaskapaiID, TanggalWaktuKeberangkatan, DurasiPenerbangan, HargaPerTiket)
VALUES ('ID-6589', 7, 9, 4, '2026-05-22 05:00:00', 90, 920000);

IF NOT EXISTS (SELECT 1 FROM JadwalPenerbangan WHERE KodePenerbangan = 'JT-0786')
INSERT INTO JadwalPenerbangan (KodePenerbangan, BandaraKeberangkatanID, BandaraTujuanID, MaskapaiID, TanggalWaktuKeberangkatan, DurasiPenerbangan, HargaPerTiket)
VALUES ('JT-0786', 7, 9, 8, '2026-05-22 05:30:00', 90, 750000);

IF NOT EXISTS (SELECT 1 FROM JadwalPenerbangan WHERE KodePenerbangan = 'JT-0646')
INSERT INTO JadwalPenerbangan (KodePenerbangan, BandaraKeberangkatanID, BandaraTujuanID, MaskapaiID, TanggalWaktuKeberangkatan, DurasiPenerbangan, HargaPerTiket)
VALUES ('JT-0646', 7, 9, 8, '2026-05-22 05:55:00', 90, 760000);

IF NOT EXISTS (SELECT 1 FROM JadwalPenerbangan WHERE KodePenerbangan = 'QG-0725')
INSERT INTO JadwalPenerbangan (KodePenerbangan, BandaraKeberangkatanID, BandaraTujuanID, MaskapaiID, TanggalWaktuKeberangkatan, DurasiPenerbangan, HargaPerTiket)
VALUES ('QG-0725', 7, 9, 6, '2026-05-22 06:00:00', 95, 800000);

IF NOT EXISTS (SELECT 1 FROM JadwalPenerbangan WHERE KodePenerbangan = 'IU-0706')
INSERT INTO JadwalPenerbangan (KodePenerbangan, BandaraKeberangkatanID, BandaraTujuanID, MaskapaiID, TanggalWaktuKeberangkatan, DurasiPenerbangan, HargaPerTiket)
VALUES ('IU-0706', 7, 9, 10, '2026-05-22 06:00:00', 60, 810000);

IF NOT EXISTS (SELECT 1 FROM JadwalPenerbangan WHERE KodePenerbangan = 'IU-0333')
INSERT INTO JadwalPenerbangan (KodePenerbangan, BandaraKeberangkatanID, BandaraTujuanID, MaskapaiID, TanggalWaktuKeberangkatan, DurasiPenerbangan, HargaPerTiket)
VALUES ('IU-0333', 7, 9, 10, '2026-05-22 06:00:00', 90, 820000);
GO

SELECT KodePenerbangan FROM JadwalPenerbangan ORDER BY TanggalWaktuKeberangkatan DESC;
GO
