import request from '@/utils/request'

export function test() {
  return request({
    url: '/api/Test/CheckPermission',
    method: 'get'
  })
}
