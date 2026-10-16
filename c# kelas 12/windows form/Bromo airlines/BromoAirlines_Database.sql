-- ============================================================
-- DATABASE SCRIPT: BromoAirlines
-- LKS Provinsi Jawa Timur 2023
-- Jalankan script ini di SQL Server Management Studio (SSMS)
-- ============================================================

-- Buat database baru
CREATE DATABASE BromoAirlines;
GO

-- Gunakan database yang baru dibuat
USE BromoAirlines;
GO

-- ============================================================
-- TABEL: Negara
-- Menyimpan data negara untuk referensi bandara
-- ============================================================
CREATE TABLE Negara (
    ID              INT             NOT NULL IDENTITY(1,1),  -- Auto increment
    Nama            VARCHAR(200)    NOT NULL,
    IbukotaNegara   VARCHAR(200)    NOT NULL,
    CONSTRAINT PK_Negara PRIMARY KEY (ID)
);
GO

-- ============================================================
-- TABEL: Bandara
-- Menyimpan data bandara
-- ============================================================
CREATE TABLE Bandara (
    ID              INT             NOT NULL IDENTITY(1,1),
    Nama            VARCHAR(200)    NOT NULL,                -- Unik, tidak boleh duplikat
    KodeIATA        VARCHAR(5)      NOT NULL,                -- Unik, 3 huruf
    Kota            VARCHAR(200)    NOT NULL,
    NegaraID        INT             NOT NULL,                -- FK ke Negara
    JumlahTerminal  INT             NOT NULL,
    Alamat          TEXT            NOT NULL,
    CONSTRAINT PK_Bandara PRIMARY KEY (ID),
    CONSTRAINT FK_Bandara_Negara FOREIGN KEY (NegaraID) REFERENCES Negara(ID),
    CONSTRAINT UQ_Bandara_Nama UNIQUE (Nama),
    CONSTRAINT UQ_Bandara_KodeIATA UNIQUE (KodeIATA)
);
GO

-- ============================================================
-- TABEL: Maskapai
-- Menyimpan data maskapai penerbangan
-- ============================================================
CREATE TABLE Maskapai (
    ID          INT             NOT NULL IDENTITY(1,1),
    Nama        VARCHAR(200)    NOT NULL,
    Perusahaan  VARCHAR(200)    NOT NULL,
    JumlahKru   INT             NOT NULL,
    Deskripsi   TEXT            NOT NULL,
    CONSTRAINT PK_Maskapai PRIMARY KEY (ID)
);
GO

-- ============================================================
-- TABEL: JadwalPenerbangan
-- Menyimpan jadwal penerbangan
-- ============================================================
CREATE TABLE JadwalPenerbangan (
    ID                          INT             NOT NULL IDENTITY(1,1),
    KodePenerbangan             VARCHAR(10)     NOT NULL,   -- Format: XX-YYYY (2 huruf, strip, 4 angka)
    BandaraKeberangkatanID      INT             NOT NULL,   -- FK ke Bandara
    BandaraTujuanID             INT             NOT NULL,   -- FK ke Bandara (harus berbeda)
    MaskapaiID                  INT             NOT NULL,   -- FK ke Maskapai
    TanggalWaktuKeberangkatan   DATETIME        NOT NULL,
    DurasiPenerbangan           INT             NOT NULL,   -- Dalam menit
    HargaPerTiket               FLOAT           NOT NULL,
    CONSTRAINT PK_JadwalPenerbangan PRIMARY KEY (ID),
    CONSTRAINT FK_JadwalPenerbangan_BandaraKeberangkatan FOREIGN KEY (BandaraKeberangkatanID) REFERENCES Bandara(ID),
    CONSTRAINT FK_JadwalPenerbangan_BandaraTujuan FOREIGN KEY (BandaraTujuanID) REFERENCES Bandara(ID),
    CONSTRAINT FK_JadwalPenerbangan_Maskapai FOREIGN KEY (MaskapaiID) REFERENCES Maskapai(ID)
);
GO

-- ============================================================
-- TABEL: KodePromo
-- Menyimpan kode promo / diskon
-- ============================================================
CREATE TABLE KodePromo (
    ID                  INT             NOT NULL IDENTITY(1,1),
    Kode                VARCHAR(100)    NOT NULL,   -- Unik, huruf kapital semua
    PersentaseDiskon    FLOAT           NOT NULL,   -- Minimal 1
    MaksimumDiskon      FLOAT           NOT NULL,   -- Minimal 1
    BerlakuSampai       DATE            NOT NULL,
    Deskripsi           TEXT            NOT NULL,
    CONSTRAINT PK_KodePromo PRIMARY KEY (ID),
    CONSTRAINT UQ_KodePromo_Kode UNIQUE (Kode)
);
GO

-- ============================================================
-- TABEL: StatusPenerbangan
-- Menyimpan jenis-jenis status penerbangan
-- ============================================================
CREATE TABLE StatusPenerbangan (
    ID      INT             NOT NULL IDENTITY(1,1),
    Nama    VARCHAR(200)    NOT NULL,
    CONSTRAINT PK_StatusPenerbangan PRIMARY KEY (ID)
);
GO

-- ============================================================
-- TABEL: PerubahanStatusJadwalPenerbangan
-- Menyimpan riwayat perubahan status penerbangan
-- ============================================================
CREATE TABLE PerubahanStatusJadwalPenerbangan (
    ID                      INT         NOT NULL IDENTITY(1,1),
    JadwalPenerbanganID     INT         NOT NULL,   -- FK ke JadwalPenerbangan
    StatusPenerbanganID     INT         NOT NULL,   -- FK ke StatusPenerbangan
    WaktuPerubahanTerjadi   DATETIME    NOT NULL,
    PerkiraanDurasiDelay    INT         NULL,       -- Boleh NULL, dalam menit (hanya untuk status Delay)
    CONSTRAINT PK_PerubahanStatus PRIMARY KEY (ID),
    CONSTRAINT FK_PerubahanStatus_Jadwal FOREIGN KEY (JadwalPenerbanganID) REFERENCES JadwalPenerbangan(ID),
    CONSTRAINT FK_PerubahanStatus_Status FOREIGN KEY (StatusPenerbanganID) REFERENCES StatusPenerbangan(ID)
);
GO

-- ============================================================
-- TABEL: Akun
-- Menyimpan data akun pengguna (Admin & Customer)
-- ============================================================
CREATE TABLE Akun (
    ID              INT             NOT NULL IDENTITY(1,1),
    Username        VARCHAR(200)    NOT NULL,   -- Unik
    Password        VARCHAR(200)    NOT NULL,   -- Minimal 8 karakter
    Nama            VARCHAR(200)    NOT NULL,
    TanggalLahir    DATE            NOT NULL,
    NomorTelepon    VARCHAR(20)     NOT NULL,   -- Angka, 10-15 digit
    MerupakanAdmin  BIT             NOT NULL DEFAULT 0,  -- 0 = Customer, 1 = Admin
    CONSTRAINT PK_Akun PRIMARY KEY (ID),
    CONSTRAINT UQ_Akun_Username UNIQUE (Username)
);
GO

-- ============================================================
-- TABEL: TransaksiHeader
-- Menyimpan header / ringkasan transaksi pembelian tiket
-- ============================================================
CREATE TABLE TransaksiHeader (
    ID                  INT         NOT NULL IDENTITY(1,1),
    AkunID              INT         NOT NULL,   -- FK ke Akun
    TanggalTransaksi    DATETIME    NOT NULL,
    JadwalPenerbanganID INT         NOT NULL,   -- FK ke JadwalPenerbangan
    JumlahPenumpang     INT         NOT NULL,
    TotalHarga          FLOAT       NOT NULL,
    KodePromoID         INT         NULL,       -- FK ke KodePromo, boleh NULL (tidak pakai promo)
    CONSTRAINT PK_TransaksiHeader PRIMARY KEY (ID),
    CONSTRAINT FK_TransaksiHeader_Akun FOREIGN KEY (AkunID) REFERENCES Akun(ID),
    CONSTRAINT FK_TransaksiHeader_Jadwal FOREIGN KEY (JadwalPenerbanganID) REFERENCES JadwalPenerbangan(ID),
    CONSTRAINT FK_TransaksiHeader_KodePromo FOREIGN KEY (KodePromoID) REFERENCES KodePromo(ID)
);
GO

-- ============================================================
-- TABEL: TransaksiDetail
-- Menyimpan detail penumpang per transaksi
-- ============================================================
CREATE TABLE TransaksiDetail (
    ID                      INT             NOT NULL IDENTITY(1,1),
    TransaksiHeaderID       INT             NOT NULL,   -- FK ke TransaksiHeader
    TitelPenumpang          VARCHAR(20)     NOT NULL,   -- Contoh: Mr, Mrs, Ms
    NamaLengkapPenumpang    VARCHAR(200)    NOT NULL,
    CONSTRAINT PK_TransaksiDetail PRIMARY KEY (ID),
    CONSTRAINT FK_TransaksiDetail_Header FOREIGN KEY (TransaksiHeaderID) REFERENCES TransaksiHeader(ID)
);
GO

-- ============================================================
-- DATA AWAL: StatusPenerbangan
-- Isi data status yang tersedia
-- ============================================================
INSERT INTO StatusPenerbangan (Nama) VALUES
('Sesuai Jadwal'),
('Delay'),
('Dibatalkan');
GO

-- ============================================================
-- DATA AWAL: Negara (beberapa negara umum)
-- ============================================================
INSERT INTO Negara (Nama, IbukotaNegara) VALUES
('Indonesia', 'Jakarta'),
('Malaysia', 'Kuala Lumpur'),
('Singapura', 'Singapura'),
('Thailand', 'Bangkok'),
('Australia', 'Canberra'),
('Jepang', 'Tokyo'),
('Korea Selatan', 'Seoul'),
('China', 'Beijing'),
('Amerika Serikat', 'Washington D.C.'),
('Inggris', 'London'),
('Belanda', 'Amsterdam'),
('Arab Saudi', 'Riyadh'),
('Uni Emirat Arab', 'Abu Dhabi'),
('India', 'New Delhi'),
('Afganistan', 'Kabul');
GO

-- ============================================================
-- DATA AWAL: Akun Admin
-- Username: admin | Password: admin123
-- ============================================================
INSERT INTO Akun (Username, Password, Nama, TanggalLahir, NomorTelepon, MerupakanAdmin)
VALUES ('admin', 'admin123', 'Administrator', '1990-01-01', '08000000000', 1);
GO

-- ============================================================
-- DATA AWAL: Maskapai (contoh data)
-- ============================================================
INSERT INTO Maskapai (Nama, Perusahaan, JumlahKru, Deskripsi) VALUES
('Adam Air', 'PT Adam Air', 16, 'Adam Air adalah maskapai penerbangan Indonesia yang beroperasi di berbagai rute domestik.'),
('Aviastar', 'PT Aviastar', 12, 'Aviastar adalah maskapai penerbangan Indonesia yang melayani rute-rute perintis.'),
('Batavia Air', 'PT Batavia Air', 6, 'Batavia Air adalah maskapai penerbangan Indonesia yang telah berhenti beroperasi.'),
('Batik Air', 'PT Batik Air', 17, 'Batik Air adalah maskapai full service milik Lion Air Group.'),
('Bourag Indonesia', 'PT Bourag Indonesia', 19, 'Bourag Indonesia adalah maskapai penerbangan Indonesia.'),
('Citilink', 'PT Citilink', 12, 'Citilink adalah maskapai bertarif rendah anak perusahaan Garuda Indonesia.'),
('Garuda Indonesia', 'PT Garuda Indonesia', 5, 'Garuda Indonesia adalah maskapai penerbangan nasional Indonesia.'),
('Lion Air', 'PT Lion Air', 14, 'Lion Air adalah maskapai penerbangan bertarif rendah terbesar di Indonesia.'),
('Pelita Air', 'PT Pelita Air', 10, 'Pelita Air adalah maskapai penerbangan Indonesia milik Pertamina.'),
('Super Air Jet', 'PT Super Air Jet', 8, 'Super Air Jet adalah maskapai penerbangan bertarif rendah yang beroperasi di Indonesia.');
GO

-- ============================================================
-- DATA AWAL: Bandara (contoh data)
-- ============================================================
DECLARE @IndonesiaID INT;
SELECT @IndonesiaID = ID FROM Negara WHERE Nama = 'Indonesia';

INSERT INTO Bandara (Nama, KodeIATA, Kota, NegaraID, JumlahTerminal, Alamat) VALUES
('Abdul Rachman Saleh', 'MLG', 'Malang', @IndonesiaID, 2, 'Jl. Mondoroko, Singosari, Malang'),
('Abresso', 'RSK', 'Manokwari', @IndonesiaID, 2, 'Manokwari, Papua Barat'),
('Achmad Yani', 'SRG', 'Semarang', @IndonesiaID, 2, 'Jl. Puad Ahmad Yani, Semarang'),
('Adi Sucipto', 'JOG', 'Yogyakarta', @IndonesiaID, 2, 'Jl. Raya Maguwo, Sleman, Yogyakarta'),
('Adisumarmo', 'SOC', 'Solo', @IndonesiaID, 2, 'Jl. Adi Sumarmo, Solo'),
('Halim Perdanakusuma', 'HLP', 'Jakarta', @IndonesiaID, 2, 'Jl. Halim Perdanakusuma, Jakarta Timur'),
('Juanda', 'SUB', 'Surabaya', @IndonesiaID, 2, 'Jl. Ir. H. Juanda, Sidoarjo'),
('Ngurah Rai', 'DPS', 'Bali', @IndonesiaID, 3, 'Tuban, Kuta, Badung, Bali'),
('Soekarno-Hatta', 'CGK', 'Tangerang', @IndonesiaID, 3, 'Jl. Lingkar Dalam, Tangerang'),
('Supadio', 'PNK', 'Pontianak', @IndonesiaID, 2, 'Jl. Adi Sucipto, Pontianak');
GO

-- ============================================================
-- DATA AWAL: KodePromo
-- ============================================================
INSERT INTO KodePromo (Kode, PersentaseDiskon, MaksimumDiskon, BerlakuSampai, Deskripsi) VALUES
('BROMOAJAYUK', 15, 100000, '2023-09-09', 'Yuk pakai diskon spesial Bromo Airlines!'),
('NAIKBROMOAI', 10, 150000, '2023-09-09', 'Bromo Airlines spesial promo naik pesawat!');
GO

PRINT 'Database BromoAirlines berhasil dibuat!';
PRINT 'Akun Admin: username=admin, password=admin123';
GO
