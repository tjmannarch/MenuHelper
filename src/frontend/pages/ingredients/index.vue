<template>
	<view class="page">
		<wd-toast />

		<!-- 搜索栏 -->
		<view class="search-bar">
			<wd-search v-model="keyword" placeholder="搜索原材料" @search="loadList" @clear="onClear" />
		</view>

		<!-- 分类 Tab -->
		<wd-tabs v-model="activeTab" @change="onTabChange">
			<wd-tab title="全部" :name="0" />
			<wd-tab v-for="cat in categories" :key="cat.value" :title="cat.label" :name="cat.value" />
		</wd-tabs>

		<!-- 列表 -->
		<scroll-view scroll-y class="list-wrap" @scrolltolower="loadMore" refresher-enabled
			@refresherrefresh="onRefresh" :refresher-triggered="refreshing">
			<view v-if="list.length === 0 && !loading" class="empty">
				<wd-status-tip image="search" tip="暂无原材料，点击右下角添加" />
			</view>

			<wd-cell-group v-else>
				<wd-cell v-for="item in list" :key="item.id" :title="item.name"
					:value="item.defaultUnitPrice != null ? `¥${item.defaultUnitPrice}/${item.unit}` : ''" is-link
					@click="goEdit(item.id)">
					<template #label>
						<view class="cell-label">
							<wd-tag size="small" :type="categoryTagType(item.category)">{{ categoryLabel(item.category)
							}}</wd-tag>
							<wd-tag size="small" type="default" plain>{{ consumptionLabel(item.consumptionType)
							}}</wd-tag>
						</view>
					</template>
				</wd-cell>
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
import { onShow } from '@dcloudio/uni-app'
import { useToast } from 'wot-design-uni'
import { api } from '@/utils/api.js'

const toast = useToast()

const keyword = ref('')
const activeTab = ref(0)
const list = ref([])
const loading = ref(false)
const refreshing = ref(false)
const pageIndex = ref(1)
const noMore = ref(false)
const PAGE_SIZE = 20

// 从接口动态加载的枚举数据
const categories = ref([])
const consumptionTypes = ref([])

async function loadMeta() {
	try {
		const res = await api.get('/api/meta/ingredients')
		categories.value = res.categories ?? []
		consumptionTypes.value = res.consumptionTypes ?? []
	} catch (e) { /* 降级使用空列表 */ }
}

const CATEGORY_TAG = { 1: 'primary', 2: 'warning', 3: 'success', 4: 'default' }

const categoryLabel = (v) => categories.value.find(c => c.value === v)?.label ?? v
const categoryTagType = (v) => CATEGORY_TAG[v] || ''
const consumptionLabel = (v) => consumptionTypes.value.find(c => c.value === v)?.label ?? v

async function loadList(reset = true) {
	if (reset) { pageIndex.value = 1; noMore.value = false }
	if (noMore.value || loading.value) return
	loading.value = true
	try {
		const params = {
			keyword: keyword.value || undefined,
			category: activeTab.value > 0 ? activeTab.value : undefined,
			pageIndex: pageIndex.value,
			pageSize: PAGE_SIZE
		}
		const res = await api.get('/api/ingredients', params)
		const items = res.items ?? []
		if (reset) {
			list.value = items
		} else {
			list.value.push(...items)
		}
		if (items.length < PAGE_SIZE) noMore.value = true
	} catch (e) {
		toast.error(e.message)
	} finally {
		loading.value = false
		refreshing.value = false
	}
}

function loadMore() {
	if (!noMore.value && !loading.value) {
		pageIndex.value++
		loadList(false)
	}
}

function onRefresh() {
	refreshing.value = true
	loadList(true)
}

function onTabChange({ name }) {
	activeTab.value = name
	loadList(true)
}

function onClear() {
	keyword.value = ''
	loadList(true)
}

function goEdit(id) {
	uni.navigateTo({ url: `/pages/ingredients/edit?id=${id || ''}` })
}

onMounted(() => {
	loadMeta()
})

// 每次从编辑页返回时刷新列表
onShow(() => {
	loadList()
})
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

.cell-label {
	display: flex;
	align-items: center;
	flex-wrap: wrap;
	margin-top: 6rpx;
	gap: 8rpx;
}

:deep(.wd-cell__value) {
	color: #f5a623;
	font-size: 26rpx;
}

.empty {
	padding-top: 120rpx;
}

.loading-tip {
	display: flex;
	align-items: center;
	justify-content: center;
	padding: 24rpx;
	gap: 12rpx;

	.loading-text {
		font-size: 24rpx;
		color: #999;
	}
}

.no-more {
	text-align: center;
	font-size: 24rpx;
	color: #ccc;
	padding: 24rpx;
}

/* FAB 悬浮按钮 */
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
