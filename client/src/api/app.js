import request from '@/utils/request'

export function GetIp() {
  return request({
    url: 'https://api.ipify.org?format=json',
    method: 'get'
  })
}
