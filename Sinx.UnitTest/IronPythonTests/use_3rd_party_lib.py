import requests
r = requests.get('http://baidu.com', auth=('user', 'pass'))
result = r.status_code