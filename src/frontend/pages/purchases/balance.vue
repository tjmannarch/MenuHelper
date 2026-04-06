<template>
  <view class="page">
    <wd-toast />

    <!-- 总欠款横幅 -->
    <view class="total-banner" v-if="totalOutstanding > 0">
      <text class="banner-label">总待结算金额</text>
      <text class="banner-amount">¥{{ totalOutstanding.toFixed(2) }}</text>
    </view>

    <scroll-view scroll-y class="list-wrap"
      refresher-enabled @refresherrefresh="onRefresh" :refresher-triggered="refreshing">
      <view v-if="list.length === 0 && !loading" class="empty">
        <wd-status-tip image="collect" tip="所有供应商账单均已结清 🎉" />
      </view>
      <wd-cell-group v-else>
        <wd-cell v-for="item in list" :key="item.supplierId" :title="item.supplierName">
          <template #label>
            <text class="unpaid-count">{{ item.unpaidPurchaseCount }} 笔未结算</text>
          </template>
          <template #right-icon>
            <view class="cell-right">
              <text class="outstanding">¥{{ item.outstandingAmount.toFixed(2) }}</text>
              <wd-button
                size="small"
                type="primary"
                :loading="settling === item.supplierId"
                @click.stop="settle(item)"
              >一键结清</wd-button>
            </view>
          </template>
        </wd-cell>
      </wd-cell-group>
      <view v-if="loading" class="loading-tip">
        <wd-loading size="20px" /><text class="loading-text">加载中...</text>
      </view>
    </scroll-view>
  </view>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { onShow } from '@dcloudio/uni-app'
import { useToast } from 'wot-design-uni'
import { api } from '@/utils/api.js'

const toast = useToast()
const list = ref([])
const loading = ref(false)
const refreshing = ref(false)
const settling = ref(null)

const totalOutstanding = computed(() =>
  list.value.reduce((sum, i) => sum + (i.outstandingAmount || 0), 0)
)

async function loadList() {
  loading.value = true
  try {
    list.value = (await api.get('/api/supplier-balances')) ?? []
  } catch (e) {
    toast.error(e.message)
  } finally {
    loading.value = false
    refreshing.value = false
  }
}

function onRefresh() { refreshing.value = true; loadList() }

async function settle(item) {
  uni.showModal({
    title: '确认结清',
    content: `确认将 ${item.supplierName} 的 ${item.unpaidPurchaseCount} 笔进货记录全部标记为已结算？共 ¥${item.outstandingAmount.toFixed(2)}`,
    success: async ({ confirm }) => {
      if (!confirm) return
      settling.value = item.supplierId
      try {
        await api.post(`/api/suppliers/${item.supplierId}/settle`, {})
        toast.success('结清成功')
        await loadList()
      } catch (e) {
        toast.error(e.message)
      } finally {
        settling.value = null
      }
    }
  })
}

onShow(() => loadList())

onMounted(() => {
  uni.setNavigationBarTitle({ title: '供应商对账' })
})
</script>

<style lang="scss" scoped>
.page { display: flex; flex-direction: column; background: #f5f5f5; min-height: 100vh; }
.total-banner {
  background: linear-gradient(135deg, #ee0a24, #c62020);
  padding: 32rpx; text-align: center;
}
.banner-label { display: block; font-size: 26rpx; color: rgba(255,255,255,0.8); margin-bottom: 8rpx; }
.banner-amount { display: block; font-size: 56rpx; font-weight: bold; color: #fff; }
.list-wrap { flex: 1; }
.unpaid-count { font-size: 24rpx; color: #999; }
.cell-right { display: flex; flex-direction: column; align-items: flex-end; gap: 12rpx; }
.outstanding { font-size: 30rpx; font-weight: bold; color: #ee0a24; }
.empty { padding-top: 120rpx; }
.loading-tip { display: flex; align-items: center; justify-content: center; padding: 24rpx; gap: 12rpx; }
.loading-text { font-size: 24rpx; color: #999; }
</style>
