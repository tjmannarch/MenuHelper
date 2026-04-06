<template>
  <view class="page">
    <wd-toast />

    <view class="amount-section">
      <text class="amount-label">今日营业额（元）</text>
      <view class="amount-input-wrap">
        <text class="currency">¥</text>
        <input
          class="amount-input"
          type="digit"
          v-model="form.amount"
          placeholder="0.00"
          placeholder-style="color:#ccc;font-size:60rpx"
          focus
        />
      </view>
    </view>

    <wd-cell-group custom-class="form-group">
      <wd-datetime-picker
        v-model="form.dateTs"
        type="date"
        label="营业日期"
        :max-date="maxDateTs"
        @confirm="onDateConfirm"
      />
      <wd-input label="备注" v-model="form.note" placeholder="今日备注（选填）" clearable />
    </wd-cell-group>

    <view class="btn-area">
      <wd-button block type="primary" :loading="saving" @click="save">保存营业额</wd-button>
    </view>
  </view>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { onShow } from '@dcloudio/uni-app'
import { useToast } from 'wot-design-uni'
import { api } from '@/utils/api.js'

const toast = useToast()
const saving = ref(false)

function formatDate(ts) {
  const d = new Date(ts)
  return `${d.getFullYear()}-${String(d.getMonth()+1).padStart(2,'0')}-${String(d.getDate()).padStart(2,'0')}`
}

const today = new Date()
today.setHours(0,0,0,0)
const maxDateTs = today.getTime()

const form = ref({
  dateTs: today.getTime(),
  dateStr: formatDate(today.getTime()),
  amount: '',
  note: ''
})

function onDateConfirm({ value }) {
  form.value.dateTs = value
  form.value.dateStr = formatDate(value)
  loadExisting()
}

async function loadExisting() {
  try {
    const res = await api.get(`/api/daily-revenues/${form.value.dateStr}`)
    if (res && res.amount > 0) {
      form.value.amount = String(res.amount)
      form.value.note = res.note ?? ''
    } else {
      form.value.amount = ''
      form.value.note = ''
    }
  } catch (e) {
    // no existing record is fine
  }
}

async function save() {
  const amount = parseFloat(form.value.amount)
  if (isNaN(amount) || amount < 0) return toast.warning('请输入有效的营业额')
  saving.value = true
  try {
    await api.post('/api/daily-revenues', {
      date: form.value.dateStr,
      amount,
      note: form.value.note.trim() || null
    })
    toast.success('保存成功')
    setTimeout(() => uni.navigateBack(), 800)
  } catch (e) {
    toast.error(e.message)
  } finally {
    saving.value = false
  }
}

onMounted(() => {
  uni.setNavigationBarTitle({ title: '录入营业额' })
  loadExisting()
})
</script>

<style lang="scss">
.page { background: #f5f5f5; min-height: 100vh; padding-bottom: 60rpx; }
.amount-section {
  background: linear-gradient(135deg, #4a90e2, #357abd);
  padding: 48rpx 40rpx 60rpx;
  text-align: center;
}
.amount-label { display: block; font-size: 28rpx; color: rgba(255,255,255,0.8); margin-bottom: 24rpx; }
.amount-input-wrap { display: flex; align-items: center; justify-content: center; }
.currency { font-size: 48rpx; color: #fff; margin-right: 8rpx; font-weight: bold; }
.amount-input {
  font-size: 80rpx; font-weight: bold; color: #fff;
  border: none; background: transparent;
  width: 400rpx; text-align: center;
  caret-color: #fff;
}
:deep(.form-group) { margin-top: -20rpx; border-radius: 16rpx 16rpx 0 0; overflow: hidden; }
.btn-area { padding: 32rpx; }
</style>
