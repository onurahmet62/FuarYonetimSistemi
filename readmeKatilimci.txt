# 🎯 Katılımcı API Dokümantasyonu

## 🎯 Genel Bakış

Fuar Yönetim Sistemi Katılımcı API'si, fuar katılımcılarının yönetimi için kullanılan RESTful API'dir. Bu API ile katılımcı ekleme, güncelleme, silme ve listeleme işlemleri yapılabilir.

---

## 🔐 Kimlik Doğrulama

API'ye erişim için JWT (JSON Web Token) kimlik doğrulaması gereklidir.

### Token Formatı
```
Authorization: Bearer <JWT_TOKEN>
```

### Gerekli Roller
- `Admin`
- `Manager` 
- `SalesPerson`

---

## 📋 Endpoint'ler

### 1. Katılımcı Listesi
```http
GET /api/Participants
```

**Açıklama:** Tüm katılımcıları listeler.

**Headers:**
```
Authorization: Bearer <JWT_TOKEN>
```

**Response (200):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fullName": "Ahmet Yılmaz",
    "email": "ahmet@example.com",
    "phone": "05551234567",
    "companyName": "ABC Şirketi",
    "address": "İstanbul, Türkiye",
    "website": "www.abc.com",
    "authFullName": "Mehmet Yetkili",
    "createDate": "2025-08-12T20:30:00Z",
    "logoFileName": "logo.jpg",
    "logoFilePath": "/uploads/logos/logo.jpg",
    "logoContentType": "image/jpeg",
    "logoFileSize": 102400,
    "logoUploadDate": "2025-08-12T20:30:00Z",
    "branches": [
      {
        "id": 1,
        "name": "İstanbul Şubesi"
      }
    ],
    "brands": [
      {
        "id": 1,
        "name": "Marka A"
      }
    ],
    "productCategories": [
      {
        "id": 1,
        "name": "Elektronik"
      }
    ],
    "exhibitedProducts": [
      {
        "id": 1,
        "name": "Ürün 1"
      }
    ],
    "representativeCompanies": [
      {
        "id": 1,
        "name": "Temsilci Firma",
        "country": "Türkiye",
        "address": "İstanbul",
        "district": "Kadıköy",
        "city": "İstanbul",
        "phone": "02161234567",
        "email": "info@temsilci.com",
        "website": "www.temsilci.com"
      }
    ],
    "logoUrl": "/api/Participants/3fa85f64-5717-4562-b3fc-2c963f66afa6/logo",
    "hasLogo": true,
    "totalBranches": 1,
    "totalBrands": 1,
    "totalProducts": 1,
    "totalRepresentatives": 1
  }
]
```

---

### 2. Katılımcı Ekleme
```http
POST /api/Participants
```

**Açıklama:** Yeni bir katılımcı ekler.

**Headers:**
```
Authorization: Bearer <JWT_TOKEN>
Content-Type: multipart/form-data
```

**Form Data:**

| Alan | Tip | Zorunlu | Açıklama |
|------|-----|---------|----------|
| `FullName` | string | ✅ | Katılımcının ad soyad bilgisi (max 150 karakter) |
| `Email` | string | ✅ | Email adresi (max 150 karakter) |
| `Phone` | string | ✅ | Telefon numarası (max 50 karakter) |
| `CompanyName` | string | ✅ | Firma adı (max 200 karakter) |
| `AuthFullName` | string | ❌ | Yetkili kişinin ad soyad bilgisi (max 150 karakter) |
| `Address` | string | ❌ | Firma adresi (max 300 karakter) |
| `Website` | string | ❌ | Website adresi (max 250 karakter) |
| `LogoFile` | file | ❌ | Logo dosyası (max 1MB, JPG/PNG/GIF) |
| `BranchesJson` | string | ❌ | Şubeler JSON formatında |
| `BrandsJson` | string | ❌ | Markalar JSON formatında |
| `ProductCategoriesJson` | string | ❌ | Ürün kategorileri JSON formatında |
| `ExhibitedProductsJson` | string | ❌ | Sergilenen ürünler JSON formatında |
| `RepresentativeCompaniesJson` | string | ❌ | Temsilci firmalar JSON formatında |

**JSON Formatları:**

**Şubeler:**
```json
[
  {"name": "İstanbul Şubesi"},
  {"name": "Ankara Şubesi"}
]
```

**Markalar:**
```json
[
  {"name": "Marka A"},
  {"name": "Marka B"}
]
```

**Ürün Kategorileri:**
```json
[
  {"name": "Elektronik"},
  {"name": "Tekstil"}
]
```

**Sergilenen Ürünler:**
```json
[
  {"name": "Ürün 1"},
  {"name": "Ürün 2"}
]
```

**Temsilci Firmalar:**
```json
[
  {
    "name": "ABC Ltd",
    "country": "Türkiye",
    "address": "İstanbul",
    "district": "Kadıköy",
    "city": "İstanbul",
    "phone": "02161234567",
    "email": "info@abc.com",
    "website": "www.abc.com"
  }
]
```

**Örnek Request (cURL):**
```bash
curl -X 'POST' \
  'https://localhost:7006/api/Participants' \
  -H 'accept: text/plain' \
  -H 'Authorization: Bearer <JWT_TOKEN>' \
  -H 'Content-Type: multipart/form-data' \
  -F 'FullName=Ahmet Yılmaz' \
  -F 'Email=ahmet@example.com' \
  -F 'Phone=05551234567' \
  -F 'CompanyName=ABC Şirketi' \
  -F 'AuthFullName=Mehmet Yetkili' \
  -F 'Address=İstanbul, Türkiye' \
  -F 'Website=www.abc.com' \
  -F 'BranchesJson=[{"name": "İstanbul Şubesi"}]' \
  -F 'BrandsJson=[{"name": "Marka A"}]' \
  -F 'ProductCategoriesJson=[{"name": "Elektronik"}]' \
  -F 'ExhibitedProductsJson=[{"name": "Ürün 1"}]' \
  -F 'RepresentativeCompaniesJson=[{"name": "ABC Ltd", "country": "Türkiye", "address": "İstanbul", "phone": "02161234567", "email": "info@abc.com"}]'
```

**Response (201):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fullName": "Ahmet Yılmaz",
  "email": "ahmet@example.com",
  "phone": "05551234567",
  "companyName": "ABC Şirketi",
  "address": "İstanbul, Türkiye",
  "website": "www.abc.com",
  "authFullName": "Mehmet Yetkili",
  "createDate": "2025-08-12T20:30:00Z",
  "logoFileName": "logo.jpg",
  "logoFilePath": "/uploads/logos/logo.jpg",
  "logoContentType": "image/jpeg",
  "logoFileSize": 102400,
  "logoUploadDate": "2025-08-12T20:30:00Z",
  "branches": [
    {
      "id": 1,
      "name": "İstanbul Şubesi"
    }
  ],
  "brands": [
    {
      "id": 1,
      "name": "Marka A"
    }
  ],
  "productCategories": [
    {
      "id": 1,
      "name": "Elektronik"
    }
  ],
  "exhibitedProducts": [
    {
      "id": 1,
      "name": "Ürün 1"
    }
  ],
  "representativeCompanies": [
    {
      "id": 1,
      "name": "ABC Ltd",
      "country": "Türkiye",
      "address": "İstanbul",
      "phone": "02161234567",
      "email": "info@abc.com",
      "website": "www.abc.com"
    }
  ],
  "logoUrl": "/api/Participants/3fa85f64-5717-4562-b3fc-2c963f66afa6/logo",
  "hasLogo": true,
  "totalBranches": 1,
  "totalBrands": 1,
  "totalProducts": 1,
  "totalRepresentatives": 1
}
```

---

### 3. Katılımcı Detayı
```http
GET /api/Participants/{id}
```

**Açıklama:** Belirtilen ID'ye sahip katılımcının detaylarını getirir.

**Path Parameters:**
- `id` (Guid): Katılımcı ID'si

**Response (200):** Yukarıdaki katılımcı detayı formatında

**Response (404):** Katılımcı bulunamadı

---

### 4. Katılımcı Güncelleme
```http
PUT /api/Participants/{id}
```

**Açıklama:** Mevcut katılımcının bilgilerini günceller.

**Headers:**
```
Authorization: Bearer <JWT_TOKEN>
Content-Type: multipart/form-data
```

**Form Data:** Katılımcı ekleme ile aynı alanlar + `RemoveLogo` (boolean)

**Response (200):** Güncellenmiş katılımcı bilgileri

**Response (404):** Katılımcı bulunamadı

---

### 5. Katılımcı Silme
```http
DELETE /api/Participants/{id}
```

**Açıklama:** Belirtilen ID'ye sahip katılımcıyı siler.

**Response (204):** Başarılı silme

**Response (404):** Katılımcı bulunamadı

---

### 6. Sayfalı Katılımcı Listesi
```http
POST /api/Participants/paged
```

**Açıklama:** Filtrelenmiş ve sayfalanmış katılımcı listesi.

**Headers:**
```
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json
```

**Request Body:**
```json
{
  "pageNumber": 1,
  "pageSize": 10,
  "searchTerm": "ahmet",
  "companyName": "ABC",
  "email": "ahmet@example.com"
}
```

**Response (200):**
```json
{
  "items": [
    // Katılımcı listesi
  ],
  "totalCount": 100,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 10
}
```

---

### 7. Logo İndirme
```http
GET /api/Participants/{id}/logo
```

**Açıklama:** Katılımcının logo dosyasını indirir.

**Response (200):** Logo dosyası

**Response (404):** Logo bulunamadı

---

### 8. Excel Export
```http
GET /api/Participants/export-excel
```

**Açıklama:** Tüm katılımcıları Excel dosyası olarak dışa aktarır.

**Response (200):** Excel dosyası (.xlsx)

---

### 9. PDF Export (Tek Katılımcı)
```http
GET /api/Participants/{id}/export-pdf
```

**Açıklama:** Belirtilen katılımcının bilgilerini PDF olarak dışa aktarır.

**Response (200):** PDF dosyası

---

### 10. PDF Export (Tüm Katılımcılar)
```http
GET /api/Participants/export-pdf
```

**Açıklama:** Tüm katılımcıları PDF olarak dışa aktarır.

**Response (200):** PDF dosyası

---

### 11. Filtrelenmiş PDF Export
```http
POST /api/Participants/export-pdf-filtered
```

**Açıklama:** Filtrelenmiş katılımcıları PDF olarak dışa aktarır.

**Request Body:** Sayfalı liste ile aynı filtre

**Response (200):** PDF dosyası

---

## ⚠️ Hata Kodları

| Kod | Açıklama |
|-----|----------|
| 200 | Başarılı |
| 201 | Oluşturuldu |
| 204 | İçerik Yok (Silme) |
| 400 | Geçersiz İstek |
| 401 | Yetkisiz Erişim |
| 404 | Bulunamadı |
| 500 | Sunucu Hatası |

---

## 🔍 Validation Kuralları

### Zorunlu Alanlar
- `FullName`: Boş olamaz, max 150 karakter
- `Email`: Geçerli email formatı, max 150 karakter
- `Phone`: Boş olamaz, max 50 karakter
- `CompanyName`: Boş olamaz, max 200 karakter

### Opsiyonel Alanlar
- `AuthFullName`: Max 150 karakter
- `Address`: Max 300 karakter
- `Website`: Max 250 karakter
- `LogoFile`: Max 1MB, sadece JPG/PNG/GIF

### JSON Validation
- JSON formatları geçerli olmalı
- Boş JSON string'ler kabul edilir
- Hatalı JSON formatı 400 hatası döner

---

## 📝 Örnek Kullanım Senaryoları

### 1. Basit Katılımcı Ekleme
```bash
curl -X POST 'https://localhost:7006/api/Participants' \
  -H 'Authorization: Bearer <JWT_TOKEN>' \
  -F 'FullName=Test Kullanıcı' \
  -F 'Email=test@example.com' \
  -F 'Phone=05551234567' \
  -F 'CompanyName=Test Şirketi'
```

### 2. Detaylı Katılımcı Ekleme
```bash
curl -X POST 'https://localhost:7006/api/Participants' \
  -H 'Authorization: Bearer <JWT_TOKEN>' \
  -F 'FullName=Ahmet Yılmaz' \
  -F 'Email=ahmet@example.com' \
  -F 'Phone=05551234567' \
  -F 'CompanyName=ABC Şirketi' \
  -F 'AuthFullName=Mehmet Yetkili' \
  -F 'Address=İstanbul, Türkiye' \
  -F 'Website=www.abc.com' \
  -F 'LogoFile=@logo.jpg' \
  -F 'BranchesJson=[{"name": "İstanbul Şubesi"}]' \
  -F 'BrandsJson=[{"name": "Marka A"}]'
```

### 3. Katılımcı Güncelleme
```bash
curl -X PUT 'https://localhost:7006/api/Participants/{id}' \
  -H 'Authorization: Bearer <JWT_TOKEN>' \
  -F 'FullName=Güncellenmiş Ad' \
  -F 'Email=yeni@example.com' \
  -F 'Phone=05551234567' \
  -F 'CompanyName=Güncellenmiş Şirket'
```

---

## 🚀 Swagger UI

API dokümantasyonuna erişim için:
```
https://localhost:7006/swagger
```

Swagger UI üzerinden tüm endpoint'leri test edebilir ve detaylı dokümantasyona erişebilirsiniz.

---

## 📞 Destek

Herhangi bir sorun yaşadığınızda lütfen sistem yöneticisi ile iletişime geçin. 