<template>
  <view class="page">
    <wd-toast />
    <wd-cell-group custom-class="form-group">
      <wd-input label="供应商名称" v-model="form.name" placeholder="请输入供应商名称" required clearable />
      <wd-input label="联系电话" v-model="form.phone" type="tel" placeholder="选填" clearable />
    </wd-cell-group>

    <view class="section-title">备注</view>
    <view class="remark-wrap">
      <wd-textarea v-model="form.remark" placeholder="负责品类说明等（选填）" :maxlength="500" show-word-limit auto-height />
    </view>

    <view class="btn-area">
      <wd-button block type="primary" :loading="saving" @click="save">保存</wd-button>
      <wd-button v-if="isEdit" block type="error" plain custom-class="delete-btn" @click="confirmDelete">删除</wd-button>
    </view>
  </view>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useToast } from 'wot-design-uni'
import { api } from '@/utils/api.js'

const toast = useToast()
const id = ref('')
const isEdit = computed(() => !!id.value)
const form = ref({ name: '', phone: '', remark: '' })
const saving = ref(false)

async function loadDetail() {
  try {
    const data = await api.get(`/api/suppliers/${id.value}`)
    form.value = { name: data.name, phone: data.phone ?? '', remark: data.remark ?? '' }
  } catch (e) {
    toast.error(e.message)
  }
}

async function save() {
  if (!form.value.name.trim()) return toast.warning('请输入供应商名称')
  saving.value = true
  try {
    const payload = {
      name: form.value.name.trim(),
      phone: form.value.phone.trim() || null,
      remark: form.value.remark.trim() || null
    }
    if (isEdit.value) {
      await api.put(`/api/suppliers/${id.value}`, payload)
      toast.success('更新成功')
    } else {
      await api.post('/api/suppliers', payload)
      toast.success('创建成功')
    }
    setTimeout(() => uni.navigateBack(), 1000)
  } catch (e) {
    toast.error(e.message)
  } finally {
    saving.value = false
  }
}

function confirmDelete() {
  uni.showModal({
    title: '确认删除',
    content: `确认删除"${form.value.name}"吗？`,
    success: async ({ confirm }) => {
      if (!confirm) return
      try {
        await api.delete(`/api/suppliers/${id.value}`)
        toast.success('已删除')
        setTimeout(() => uni.navigateBack(), 800)
      } catch (e) {
        toast.error(e.message)
      }
    }
  })
}

onMounted(() => {
  const pages = getCurrentPages()
  const current = pages[pages.length - 1]
  id.value = current.$page?.options?.id || ''
  uni.setNavigationBarTitle({ title: isEdit.value ? '编辑供应商' : '新增供应商' })
  if (isEdit.value) loadDetail()
})
</script>

<style lang="scss">
.page { background: #f5f5f5; min-height: 100vh; padding-bottom: 60rpx; }
:deep(.form-group) { margin-bottom: 16rpx; }
.section-title { font-size: 26rpx; color: #999; padding: 24rpx 32rpx 12rpx; }
.remark-wrap { background: #fff; margin: 0 0 16rpx; padding: 16rpx 32rpx; }
.btn-area { padding: 24rpx 32rpx; :deep(.delete-btn) { margin-top: 16rpx; } }
</style>
