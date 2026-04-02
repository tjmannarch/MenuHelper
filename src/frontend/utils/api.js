const BASE_URL = 'http://localhost:5511'

/**
 * 封装 uni.request，返回 Promise<data>
 * 自动处理 ResponseData 包装格式 { code, data, message }
 */
function request(method, url, data = {}) {
  return new Promise((resolve, reject) => {
    const isGet = method === 'GET'
    uni.request({
      url: BASE_URL + url,
      method,
      data: isGet ? undefined : data,
      header: isGet ? {} : { 'Content-Type': 'application/json' },
      success(res) {
        const body = res.data
        if (res.statusCode >= 200 && res.statusCode < 300) {
          resolve(body?.data ?? body)
        } else {
          const msg = body?.message || `请求失败 (${res.statusCode})`
          reject(new Error(msg))
        }
      },
      fail(err) {
        reject(new Error(err.errMsg || '网络错误'))
      }
    })
  })
}

export const api = {
  get: (url, params = {}) => {
    const query = Object.entries(params)
      .filter(([, v]) => v !== null && v !== undefined && v !== '')
      .map(([k, v]) => `${k}=${encodeURIComponent(v)}`)
      .join('&')
    return request('GET', query ? `${url}?${query}` : url)
  },
  post:   (url, data) => request('POST',   url, data),
  put:    (url, data) => request('PUT',    url, data),
  delete: (url)       => request('DELETE', url),
}
