# PIP Distance
Indicator overlay untuk cTrader (cAlgo) yang membantu trader menentukan level **Base → SL → Entry → Min Retrace → TP** secara visual berdasarkan profile Renko tertentu.

---
<img width="891" height="888" alt="cT_cs_1184089_XAUUSD_2026-09-16_22-17-39" src="https://github.com/user-attachments/assets/01ca1375-ed63-45f6-8d73-7fb73cd375ab" />

---

## Fitur Utama

- **4 Profile Renko** dengan parameter lot, SL, dan Entry yang sudah disesuaikan
- **Mode Long / Short** (bisa diganti dengan tombol `M`)
- **Garis Minimum Retrace** (jarak = ukuran Renko yang aktif)
  - Long → di bawah Entry
  - Short → di atas Entry
- **Risk:Reward tetap 1:10** (TP = 10 × jarak Entry)
- Dashboard interaktif di pojok kiri atas (bisa di-toggle per item)
- Warna setiap garis bisa diganti melalui parameter
- Klik chart untuk set Base, `Ctrl + Klik` untuk hapus semua garis

---

## Cara Penggunaan

1. Pasang indicator ke chart.
2. Pilih **Renko Profile** dan **Mode** (Long/Short) di parameter.
3. **Klik** di chart untuk menempatkan garis Base.
4. Garis SL, Entry, Min Retrace, dan TP akan otomatis digambar sesuai jarak yang ditentukan.
5. Tekan tombol **`M`** untuk mengganti arah (Long ↔ Short).
6. `Ctrl + Klik` untuk menghapus semua garis (jika opsi *Require Ctrl to Delete* aktif).

---

## Profile Renko

| Profile       | Renko Size | Lot Rekomendasi | Jarak SL (dari Base) | Jarak Entry (dari SL) | Jarak TP (dari Entry) |
|---------------|------------|------------------|-----------------------|------------------------|------------------------|
| Profile_7ATR  | 4 pips     | 0.30             | 10 pips               | 30 pips                | 300 pips               |
| Profile_14ATR | 8 pips     | 0.15             | 20 pips               | 60 pips                | 600 pips               |
| Profile_28ATR | 15 pips    | 0.07             | 45 pips               | 130 pips               | 1300 pips              |
| Profile_56ATR | 30 pips    | 0.03             | 90 pips               | 300 pips               | 3000 pips              |

> **Catatan:** Jarak Min Retrace selalu sama dengan ukuran Renko yang sedang digunakan.

---

## Parameter

### Profile
- **Renko Profile** – Pilih salah satu dari 4 profile di atas
- **Mode** – Long atau Short

### Lines
- **Require Ctrl to Delete** – Jika `true`, harus menekan Ctrl saat klik untuk menghapus garis

### Visual
- **Base Line Style** – Style garis Base (default: Dots)
- **Line Thickness** – Ketebalan garis (1–5)
- **Label Font Size** – Ukuran font label
- **Label Offset from Right (bars)** – Jarak label dari kanan chart

### Colors
- **Base Line Color** (default: Gold)
- **SL Line Color** (default: Red)
- **Entry Line Color** (default: LimeGreen)
- **TP Line Color** (default: DodgerBlue)
- **Retrace Line Color** (default: Orange)

### Dashboard Display
Toggles untuk menampilkan/menyembunyikan bagian-bagian dashboard:
- Show Profile & Mode
- Show Renko Size
- Show Recommended Lot
- Show SL Distance
- Show Entry Distance
- Show TP Distance
- Show Risk:Reward Ratio
- Show Instructions [M/Click]

---

## Penjelasan Garis

| Garis          | Warna Default | Keterangan                                      |
|----------------|---------------|--------------------------------------------------|
| **Base**       | Gold          | Titik acuan yang diklik user                     |
| **SL**         | Red           | Stop Loss                                        |
| **Entry**      | LimeGreen     | Level entry yang diharapkan                      |
| **Min Retrace**| Orange        | Level minimum harga harus retrace sebelum entry  |
| **TP**         | DodgerBlue    | Take Profit (10× jarak Entry)                    |

---

## Keyboard & Mouse Shortcuts

| Aksi                  | Cara                          |
|-----------------------|-------------------------------|
| Set Base Line         | Klik kiri di chart            |
| Hapus semua garis     | `Ctrl + Klik`                 |
| Ganti Mode Long/Short | Tekan tombol **`M`**          |

---

## Catatan

- Indicator ini **hanya visual**. Tidak membuka order otomatis.
- Pastikan timeframe / Renko size chart sesuai dengan profile yang dipilih agar jarak pip akurat.
- Label akan otomatis menyesuaikan posisi dan warna garis.
- Dashboard dapat dikustomisasi sepenuhnya melalui parameter *Dashboard Display*.

---

**Versi:** 1.2
