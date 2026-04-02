<template>
	<view class="page">
		<wd-toast />

		<!-- 搜索栏 -->
		<view class="search-bar">
			<wd-search v-model="keyword" placeholder="搜索菜品" @search="loadList" @clear="onClear" />
		</view>

		<!-- 列表 -->
		<scroll-view scroll-y class="list-wrap" @scrolltolower="loadMore" refresher-enabled @refresherrefresh="onRefresh" :refresher-triggered="refreshing">
			<view v-if="list.length === 0 && !loading" class="empty">
				<wd-status-tip image="search" tip="暂无菜品，点击右下角添加" />
			</view>

			<wd-cell-group v-else>
				<wd-cell
					v-for="item in list"
					:key="item.id"
					:title="item.name"
					:value="`${item.ingredientCount} 种原材料`"
					is-link
					icon="view-list"
					@click="goEdit(item.id)"
				/>
			</wd-cell-group>

			<view v-if="loading" class="loading-tip">
				<wd-loading size="20px" />
				<text class="loading-text">加载中...</text>
			</view>
			<view v-if="noMore && list.length > 0" class="no-more">没有更多了</view>
		</scroll-view>

		<!-- 新增 FAB -->
		<view class="fab" @click="goEdit(null)">
			<wd-icon name="add" size="28px" color="#fff" />
		</view>
	</view>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useToast } from 'wot-design-uni'
import { api } from '@/utils/api.js'

const toast = useToast()
const keyword = ref('')
const list = ref([])
const loading = ref(false)
const refreshing = ref(false)
const pageIndex = ref(1)
const noMore = ref(false)
const PAGE_SIZE = 20

async function loadList(reset = true) {
  if (reset) { pageIndex.value = 1; noMore.value = false }
  if (noMore.value || loading.value) return
  loading.value = true
  try {
    const res = await api.get('/api/dishes', {
      keyword: keyword.value || undefined,
      pageIndex: pageIndex.value,
      pageSize: PAGE_SIZE
    })
    const items = res.items ?? []
    list.value = reset ? items : [...list.value, ...items]
    if (items.length < PAGE_SIZE) noMore.value = true
  } catch (e) {
    toast.error(e.message)
  } finally {
    loading.value = false
    refreshing.value = false
  }
}

function loadMore() {
  if (!noMore.value && !loading.value) { pageIndex.value++; loadList(false) }
}

function onRefresh() { refreshing.value = true; loadList(true) }
function onClear() { keyword.value = ''; loadList(true) }
function goEdit(id) { uni.navigateTo({ url: `/pages/dishes/edit?id=${id || ''}` }) }

onMounted(() => loadList())
</script>

<style lang="scss">
.page {
	display: flex;
	flex-direction: column;
	height: 100vh;
	background: #f5f5f5;
}

.search-bar {
	background: #fff;
	padding: 8rpx 16rpx;
}

.list-wrap {
	flex: 1;
	overflow: hidden;
}

.empty { padding-top: 120rpx; }

.loading-tip {
	display: flex;
	align-items: center;
	justify-content: center;
	padding: 24rpx;
	gap: 12rpx;
	.loading-text { font-size: 24rpx; color: #999; }
}

.no-more {
	text-align: center;
	font-size: 24rpx;
	color: #ccc;
	padding: 24rpx;
}

.fab {
	position: fixed;
	right: 40rpx;
	bottom: 80rpx;
	width: 100rpx;
	height: 100rpx;
	border-radius: 50%;
	background: #4a90e2;
	display: flex;
	align-items: center;
	justify-content: center;
	box-shadow: 0 4rpx 20rpx rgba(74, 144, 226, 0.5);
}
</style>
